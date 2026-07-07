namespace NoteScalerTests.Services
{
	using NoteScaler.Services;
	using System;
	using System.IO;
	using System.Linq;
	using Xunit;

	public class StringInstrumentDefinitionLoaderTests : IDisposable
	{
		private readonly string testDirectory;

		public StringInstrumentDefinitionLoaderTests()
		{
			testDirectory = Path.Combine(Path.GetTempPath(), $"StringInstrumentDefinitionLoaderTests_{Guid.NewGuid():N}");
			Directory.CreateDirectory(testDirectory);
		}

		[Fact]
		public void Load_WhenDefinitionFileIsValid_ReturnsInstruments()
		{
			var path = WriteJson("valid-instruments.json", @"{
				""instruments"": [
					{
						""name"": ""Seven String Drop A"",
						""strings"": 7,
						""frets"": 24,
						""capo"": 2,
						""openStrings"": [
							{ ""number"": 1, ""note"": ""E4"" },
							{ ""number"": 2, ""note"": ""B3"" },
							{ ""number"": 3, ""note"": ""G3"" },
							{ ""number"": 4, ""note"": ""D3"" },
							{ ""number"": 5, ""note"": ""A2"" },
							{ ""number"": 6, ""note"": ""E2"" },
							{ ""number"": 7, ""note"": ""A1"" }
						]
					}
				]
			}");
			var loader = new StringInstrumentDefinitionLoader();

			var result = loader.Load(path);

			Assert.True(result.Success, result.Error);
			var instrument = Assert.Single(result.Instruments);
			Assert.Equal("Seven String Drop A", instrument.Name);
			Assert.Equal(7, instrument.NumberOfStrings);
			Assert.Equal(24, instrument.Frets);
			Assert.Equal(2, instrument.EffectiveCapo);
			Assert.Equal(new[] { "E4", "B3", "G3", "D3", "A2", "E2", "A1" }, instrument.GetOpenStringNotesByStringNumber());
		}

		[Fact]
		public void Load_WhenFileIsMissing_ReturnsControlledError()
		{
			var loader = new StringInstrumentDefinitionLoader();

			var result = loader.Load(Path.Combine(testDirectory, "missing.json"));

			Assert.False(result.Success);
			Assert.Contains("file not found", result.Error);
		}

		[Fact]
		public void Load_WhenJsonIsMalformed_ReturnsControlledError()
		{
			var path = WriteJson("malformed.json", "not json");
			var loader = new StringInstrumentDefinitionLoader();

			var result = loader.Load(path);

			Assert.False(result.Success);
			Assert.Contains("Unable to load", result.Error);
		}

		[Fact]
		public void Load_WhenStringCountDoesNotMatchOpenStrings_ReturnsControlledError()
		{
			var path = WriteJson("missing-string.json", @"{
				""instruments"": [
					{
						""name"": ""Broken"",
						""strings"": 2,
						""frets"": 24,
						""openStrings"": [
							{ ""number"": 1, ""note"": ""E4"" }
						]
					}
				]
			}");
			var loader = new StringInstrumentDefinitionLoader();

			var result = loader.Load(path);

			Assert.False(result.Success);
			Assert.Contains("exactly 2 open strings", result.Error);
		}

		[Fact]
		public void Load_WhenStringNumbersAreDuplicated_ReturnsControlledError()
		{
			var path = WriteJson("duplicate-string.json", @"{
				""instruments"": [
					{
						""name"": ""Broken"",
						""strings"": 2,
						""frets"": 24,
						""openStrings"": [
							{ ""number"": 1, ""note"": ""E4"" },
							{ ""number"": 1, ""note"": ""B3"" }
						]
					}
				]
			}");
			var loader = new StringInstrumentDefinitionLoader();

			var result = loader.Load(path);

			Assert.False(result.Success);
			Assert.Contains("duplicate string number 1", result.Error);
		}

		[Fact]
		public void Load_WhenOpenStringNoteIsUnsupported_ReturnsControlledError()
		{
			var path = WriteJson("unsupported-note.json", @"{
				""instruments"": [
					{
						""name"": ""Broken"",
						""strings"": 1,
						""frets"": 24,
						""openStrings"": [
							{ ""number"": 1, ""note"": ""H4"" }
						]
					}
				]
			}");
			var loader = new StringInstrumentDefinitionLoader();

			var result = loader.Load(path);

			Assert.False(result.Success);
			Assert.Contains("unsupported note H4", result.Error);
		}

		public void Dispose()
		{
			if (Directory.Exists(testDirectory))
			{
				Directory.Delete(testDirectory, true);
			}
		}

		private string WriteJson(string fileName, string json)
		{
			var path = Path.Combine(testDirectory, fileName);
			File.WriteAllText(path, json);
			return path;
		}
	}
}
