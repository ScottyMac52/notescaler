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
		public void Load_WhenValidGuitarTabMakerGtabExistsInGtabDirectory_ReturnsTablature()
		{
			CreateGtabFile("mary.gtab", CreateGuitarTabMakerGtab("Mary", 120, "[[{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"2\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"}],[{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"}],[{\"p\":\"—\",\"s\":\"\"},{\"p\":\"0\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"}]]"));
			var loader = new GtabLoader();

			var result = loader.Load("mary.gtab", out var errorString, out var tablature);

			Assert.True(result, errorString);
			Assert.Null(errorString);
			Assert.Equal("Mary", tablature.Name);
			Assert.Equal(500, tablature.Speed);
			Assert.Equal("Standard", tablature.Tuning);
			Assert.Equal("4-2-2,5-0", tablature.TabString);
			Assert.Equal(1, tablature.Repeat);
			Assert.Equal(6, tablature.NumberOfStrings);
		}

		[Fact]
		public void Load_WhenGtabColumnContainsMultipleFrets_ConvertsColumnToChordGroup()
		{
			CreateGtabFile("chord.gtab", CreateGuitarTabMakerGtab("Chord", 120, "[[{\"p\":\"—\",\"s\":\"\"},{\"p\":\"0\",\"s\":\"\"},{\"p\":\"2\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"}]]"));
			var loader = new GtabLoader();

			var result = loader.Load("chord.gtab", out var errorString, out var tablature);

			Assert.True(result, errorString);
			Assert.Equal("5-0|4-2", tablature.TabString);
		}

		[Fact]
		public void Load_WhenExtensionIsOmitted_AddsGtabExtension()
		{
			CreateGtabFile("mary.gtab", CreateGuitarTabMakerGtab("Mary", 120, "[[{\"p\":\"—\",\"s\":\"\"},{\"p\":\"0\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"}]]"));
			var loader = new GtabLoader();

			var result = loader.Load("mary", out var errorString, out var tablature);

			Assert.True(result, errorString);
			Assert.Equal("Mary", tablature.Name);
		}

		[Fact]
		public void Load_WhenTuningIsUnsupported_ReturnsValidationError()
		{
			CreateGtabFile("weird.gtab", "{\"cFret\":0,\"title\":\"Weird\",\"tempo\":120,\"stringNotes\":[\"Q\",\"A\",\"D\",\"G\",\"B\",\"E\"],\"version\":5,\"lyricSize\":100,\"tabRows\":[{\"columns\":[[{\"p\":\"—\",\"s\":\"\"},{\"p\":\"0\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"},{\"p\":\"—\",\"s\":\"\"}]],\"columnHeaders\":[],\"lyricLines\":[],\"lyrics\":\"\"}]} ");
			var loader = new GtabLoader();

			var result = loader.Load("weird.gtab", out var errorString, out var tablature);

			Assert.False(result);
			Assert.Null(tablature);
			Assert.Contains("Unsupported .gtab tuning", errorString);
		}

		[Fact]
		public void Load_WhenRequiredFieldIsMissing_ReturnsValidationError()
		{
			CreateGtabFile("invalid.gtab", "{\"cFret\":0,\"title\":\"Invalid\",\"tempo\":120,\"version\":5,\"tabRows\":[]}");
			var loader = new GtabLoader();

			var result = loader.Load("invalid.gtab", out var errorString, out var tablature);

			Assert.False(result);
			Assert.Null(tablature);
			Assert.Contains("stringNotes", errorString);
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

		private static string CreateGuitarTabMakerGtab(string title, int tempo, string columns)
		{
			return $"{{\"cFret\":0,\"title\":\"{title}\",\"tempo\":{tempo},\"stringNotes\":[\"E\",\"A\",\"D\",\"G\",\"B\",\"E\"],\"version\":5,\"lyricSize\":100,\"tabRows\":[{{\"lyricLines\":[],\"columnHeaders\":[],\"columns\":{columns},\"lyrics\":\"\"}}]}}";
		}
	}
}
