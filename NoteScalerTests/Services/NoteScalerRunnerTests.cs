namespace NoteScalerTests.Services
{
	using NoteScaler.Config;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using NoteScaler.Services.Interfaces;
	using NoteScalerTests.Support;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Xunit;

	public class NoteScalerRunnerTests : IDisposable
	{
		private readonly string originalCurrentDirectory;
		private readonly string testDirectory;

		public NoteScalerRunnerTests()
		{
			originalCurrentDirectory = Environment.CurrentDirectory;
			testDirectory = Path.Combine(Path.GetTempPath(), $"NoteScalerRunnerTests_{Guid.NewGuid():N}");
			Directory.CreateDirectory(testDirectory);
			Environment.CurrentDirectory = testDirectory;
		}

		public static IEnumerable<object[]> SupportedCommandLinePaths => new[]
		{
			new object[] { Array.Empty<string>() },
			new object[] { new[] { "--note", "Nope" } },
			new object[] { new[] { "--file", "missing-song" } },
			new object[] { new[] { "--tab", "missing-tab" } }
		};

		[Theory]
		[MemberData(nameof(SupportedCommandLinePaths))]
		public void Run_ReturnsSuccessForSupportedCommandLinePaths(string[] args)
		{
			var harness = CreateHarness();

			var result = harness.Runner.Run(args);

			Assert.Equal(0, result);
		}

		[Theory]
		[InlineData("--file", "missing-song", "File not found")]
		[InlineData("--tab", "missing-tab", "File not found")]
		[InlineData("--note", "Nope", "Nope is NOT a valid note!")]
		public void Run_WritesExpectedErrorsForInvalidInputs(string option, string value, string expectedMessage)
		{
			var harness = CreateHarness();

			harness.Runner.Run(new[] { option, value });

			Assert.Contains(harness.Console.Messages, message => message.Contains(expectedMessage));
		}

		[Fact]
		public void Run_PlaysValidNoteAndWritesNoteDetails()
		{
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--note", "C", "--speed", "1" });

			Assert.Contains(harness.Console.Messages, message => message.Contains("is the current note"));
			Assert.Contains(harness.Console.Messages, message => message.Contains("Playing Major Scale"));
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_WritesPreWaitMessageWhenPreWaitIsConfigured()
		{
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--speed", "1", "--prewait", "1" });

			Assert.Contains(harness.Console.Messages, message => message.Contains("Pausing"));
		}

		[Fact]
		public void Run_LoadsAndPlaysSongFileWithSelectedKeyAndReversePlayback()
		{
			CreateSongFile("runner-song");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--file", "runner-song", "--key", "Alt" });

			Assert.NotNull(harness.Factory.CreatedSequence);
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_LoadsSongByDefaultKeyWhenNoKeyIsRequested()
		{
			CreateDefaultKeySongFile("runner-default-song");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--file", "runner-default-song" });

			Assert.NotNull(harness.Factory.CreatedSequence);
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_LoadsSongItselfWhenNoKeyCollectionExists()
		{
			CreateSongWithoutKeyCollection("runner-simple-song");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--file", "runner-simple-song" });

			Assert.NotNull(harness.Factory.CreatedSequence);
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_LoadsAndPlaysTabFileWithDefaultVersion()
		{
			CreateTabFile("runner-tab");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--tab", "runner-tab" });

			Assert.NotNull(harness.Factory.CreatedSequence);
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_LoadsAndPlaysTabFileWithCustomStringInstrument()
		{
			CreateSevenStringTabFile("runner-seven-string-tab");
			var instrumentDefinitionsPath = CreateSevenStringInstrumentDefinitionFile("custom-instruments.json");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--tab", "runner-seven-string-tab", "--string-instruments", instrumentDefinitionsPath, "--string-instrument", "Seven String Drop A" });

			Assert.Contains(harness.Console.Messages, message => message.Contains("Using string instrument: Seven String Drop A"));
			Assert.NotNull(harness.Factory.CreatedSequence);
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_WhenMultipleCustomStringInstrumentsAreAvailableWithoutSelection_WritesError()
		{
			CreateTabFile("runner-tab");
			var instrumentDefinitionsPath = CreateMultipleInstrumentDefinitionFile("multiple-instruments.json");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--tab", "runner-tab", "--string-instruments", instrumentDefinitionsPath });

			Assert.Contains(harness.Console.Messages, message => message.Contains("Use --string-instrument to choose one"));
		}

		public void Dispose()
		{
			Environment.CurrentDirectory = originalCurrentDirectory;
			if (Directory.Exists(testDirectory))
			{
				Directory.Delete(testDirectory, true);
			}
		}

		private static Harness CreateHarness()
		{
			var player = new TestPlayer();
			var console = new TestConsoleOutputService();
			var sequenceFactory = new TestPlayableSequenceFactory(player);
			var runner = new NoteScalerRunner(
				new CommandLineOptionsService(),
				sequenceFactory,
				new StringInstrumentFactory(),
				console);

			return new Harness(runner, console, sequenceFactory, player);
		}

		private static void CreateSongFile(string fileName)
		{
			Directory.CreateDirectory("Songs");
			File.WriteAllText(Path.Combine("Songs", $"{fileName}.json"), "{\"keyName\":\"Main\",\"sequence\":\"C,E\",\"reverse\":true,\"keys\":[{\"keyName\":\"Alt\",\"sequence\":\"D,F\"}]}");
		}

		private static void CreateDefaultKeySongFile(string fileName)
		{
			Directory.CreateDirectory("Songs");
			File.WriteAllText(Path.Combine("Songs", $"{fileName}.json"), "{\"keyName\":\"Main\",\"sequence\":\"C,E\",\"reverse\":false,\"keys\":[{\"keyName\":\"Main\",\"sequence\":\"A,B\"}]}");
		}

		private static void CreateSongWithoutKeyCollection(string fileName)
		{
			Directory.CreateDirectory("Songs");
			File.WriteAllText(Path.Combine("Songs", $"{fileName}.json"), "{\"keyName\":\"Main\",\"sequence\":\"C,E\",\"reverse\":false}");
		}

		private static void CreateTabFile(string fileName)
		{
			Directory.CreateDirectory("Tabs");
			File.WriteAllText(Path.Combine("Tabs", $"{fileName}.json"), "{\"name\":\"Tab\",\"speed\":1000,\"tab\":\"1-0\",\"tuning\":\"Standard\",\"repeat\":1,\"default\":\"Lead\",\"versions\":[{\"name\":\"Lead\",\"speed\":0,\"tab\":\"1-0,2-1\",\"tuning\":\"Standard\"}]}");
		}

		private static void CreateSevenStringTabFile(string fileName)
		{
			Directory.CreateDirectory("Tabs");
			File.WriteAllText(Path.Combine("Tabs", $"{fileName}.json"), "{\"name\":\"Seven String Tab\",\"speed\":1000,\"strings\":7,\"tab\":\"7-0\",\"tuning\":\"Standard\",\"repeat\":1,\"default\":\"Lead\",\"versions\":[{\"name\":\"Lead\",\"speed\":0,\"tab\":\"7-0,1-0\",\"tuning\":\"Standard\"}]}");
		}

		private static string CreateSevenStringInstrumentDefinitionFile(string fileName)
		{
			var path = Path.Combine(Environment.CurrentDirectory, fileName);
			File.WriteAllText(path, @"{
				""instruments"": [
					{
						""name"": ""Seven String Drop A"",
						""strings"": 7,
						""frets"": 24,
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
			return path;
		}

		private static string CreateMultipleInstrumentDefinitionFile(string fileName)
		{
			var path = Path.Combine(Environment.CurrentDirectory, fileName);
			File.WriteAllText(path, @"{
				""instruments"": [
					{
						""name"": ""Six String"",
						""strings"": 6,
						""frets"": 24,
						""openStrings"": [
							{ ""number"": 1, ""note"": ""E4"" },
							{ ""number"": 2, ""note"": ""B3"" },
							{ ""number"": 3, ""note"": ""G3"" },
							{ ""number"": 4, ""note"": ""D3"" },
							{ ""number"": 5, ""note"": ""A2"" },
							{ ""number"": 6, ""note"": ""E2"" }
						]
					},
					{
						""name"": ""Seven String"",
						""strings"": 7,
						""frets"": 24,
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
			return path;
		}

		private sealed class Harness
		{
			public Harness(NoteScalerRunner runner, TestConsoleOutputService console, TestPlayableSequenceFactory factory, TestPlayer player)
			{
				Runner = runner;
				Console = console;
				Factory = factory;
				Player = player;
			}

			public NoteScalerRunner Runner { get; }
			public TestConsoleOutputService Console { get; }
			public TestPlayableSequenceFactory Factory { get; }
			public TestPlayer Player { get; }
		}

		private sealed class TestPlayableSequenceFactory : IPlayableSequenceFactory
		{
			private readonly TestPlayer player;

			public TestPlayableSequenceFactory(TestPlayer player)
			{
				this.player = player;
			}

			public PlayableSequence CreatedSequence { get; private set; }

			public PlayableSequence Create(NoteScalerOptions options, int a4Reference)
			{
				CreatedSequence = new PlayableSequence(() => player, () => new Guitar())
				{
					MeasureTime = options.Speed.GetValueOrDefault(),
					Octave = options.Octave.GetValueOrDefault(),
					A4Reference = a4Reference,
					InstrumentType = options.Instrument
				};
				CreatedSequence.ConvertSongNotesToNoteSequence(new SongKey("Seed", "C"));
				return CreatedSequence;
			}
		}
	}
}
