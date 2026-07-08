namespace NoteScalerTests.Services
{
	using NoteScaler.Services;
	using System;
	using System.IO;
	using Xunit;

	public sealed class GtabLoaderTests : IDisposable
	{
		private readonly string originalCurrentDirectory;
		private readonly string testDirectory;

		public GtabLoaderTests()
		{
			originalCurrentDirectory = Environment.CurrentDirectory;
			testDirectory = Path.Combine(Path.GetTempPath(), $"GtabLoaderTests_{Guid.NewGuid():N}");
			Directory.CreateDirectory(testDirectory);
			Environment.CurrentDirectory = testDirectory;
		}

		[Fact]
		public void Load_WhenValidGtabExistsInGtabDirectory_ReturnsTablature()
		{
			CreateGtabFile("mary.gtab", "{\"schemaVersion\":1,\"name\":\"Mary\",\"speed\":1500,\"tuning\":\"Standard\",\"tab\":\"1-0,2-1\",\"repeat\":2,\"strings\":6}");
			var loader = new GtabLoader();

			var result = loader.Load("mary.gtab", out var errorString, out var tablature);

			Assert.True(result, errorString);
			Assert.Null(errorString);
			Assert.Equal("Mary", tablature.Name);
			Assert.Equal(1500, tablature.Speed);
			Assert.Equal("Standard", tablature.Tuning);
			Assert.Equal("1-0,2-1", tablature.TabString);
			Assert.Equal(2, tablature.Repeat);
			Assert.Equal(6, tablature.NumberOfStrings);
		}

		[Fact]
		public void Load_WhenExtensionIsOmitted_AddsGtabExtension()
		{
			CreateGtabFile("mary.gtab", "{\"schemaVersion\":1,\"name\":\"Mary\",\"speed\":1500,\"tuning\":\"Standard\",\"tab\":\"1-0\"}");
			var loader = new GtabLoader();

			var result = loader.Load("mary", out var errorString, out var tablature);

			Assert.True(result, errorString);
			Assert.Equal("Mary", tablature.Name);
		}

		[Fact]
		public void Load_WhenSchemaVersionIsUnsupported_ReturnsValidationError()
		{
			CreateGtabFile("future.gtab", "{\"schemaVersion\":2,\"name\":\"Future\",\"speed\":1500,\"tuning\":\"Standard\",\"tab\":\"1-0\"}");
			var loader = new GtabLoader();

			var result = loader.Load("future.gtab", out var errorString, out var tablature);

			Assert.False(result);
			Assert.Null(tablature);
			Assert.Contains("Unsupported .gtab schemaVersion", errorString);
		}

		[Fact]
		public void Load_WhenRequiredFieldIsMissing_ReturnsValidationError()
		{
			CreateGtabFile("invalid.gtab", "{\"schemaVersion\":1,\"name\":\"Invalid\",\"speed\":1500,\"tab\":\"1-0\"}");
			var loader = new GtabLoader();

			var result = loader.Load("invalid.gtab", out var errorString, out var tablature);

			Assert.False(result);
			Assert.Null(tablature);
			Assert.Contains("tuning", errorString, StringComparison.InvariantCultureIgnoreCase);
		}

		[Fact]
		public void Load_WhenFileDoesNotExist_ReturnsFileNotFoundError()
		{
			var loader = new GtabLoader();

			var result = loader.Load("missing.gtab", out var errorString, out var tablature);

			Assert.False(result);
			Assert.Null(tablature);
			Assert.Contains("File not found", errorString);
		}

		public void Dispose()
		{
			Environment.CurrentDirectory = originalCurrentDirectory;
			if (Directory.Exists(testDirectory))
			{
				Directory.Delete(testDirectory, true);
			}
		}

		private static void CreateGtabFile(string fileName, string contents)
		{
			Directory.CreateDirectory("GTabs");
			File.WriteAllText(Path.Combine("GTabs", fileName), contents);
		}
	}
}
