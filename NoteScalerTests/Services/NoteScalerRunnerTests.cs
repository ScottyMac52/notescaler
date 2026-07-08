namespace NoteScalerTests.Services
{
	using Newtonsoft.Json;
	using NoteScaler.Config;
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using NoteScaler.Services.Interfaces;
	using NoteScalerTests.Support;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
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
			new object[] { new[] { "--tab", "missing-tab" } },
			new object[] { new[] { "--gtab", "missing-gtab" } }
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
		[InlineData("--gtab", "missing-gtab", "File not found")]
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
			CreateTabFile("runner-tab", "Standard");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--tab", "runner-tab" });

			Assert.Contains(harness.Console.Messages, message => message.Contains("Using string instrument: Standard"));
			Assert.NotNull(harness.Factory.CreatedSequence);
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_LoadsAndPlaysGtabFile()
		{
			CreateGtabFile("runner-gtab.gtab", "Standard");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--gtab", "runner-gtab.gtab" });

			Assert.Contains(harness.Console.Messages, message => message.Contains("Using string instrument: Standard"));
			Assert.NotNull(harness.Factory.CreatedSequence);
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_WhenExportMidiIsRequestedForGtabFile_WritesMidiFileAndKeepsGtabPlayback()
		{
			CreateGtabFile("runner-midi-gtab.gtab", "Standard");
			var outputPath = Path.Combine(testDirectory, "runner-midi-gtab.mid");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--gtab", "runner-midi-gtab.gtab", "--export-midi", outputPath });

			Assert.True(File.Exists(outputPath));
			Assert.Contains(harness.Console.Messages, message => message.Contains("Exported MIDI"));
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_LoadsAndPlaysTabFileWithEmbeddedBaseStringInstrument()
		{
			CreateSevenStringTabFile("runner-seven-string-tab");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--tab", "runner-seven-string-tab" });

			Assert.Contains(harness.Console.Messages, message => message.Contains("Using string instrument: 7 String Drop A"));
			Assert.NotNull(harness.Factory.CreatedSequence);
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_LoadsAndPlaysTabFileWithExternalStringInstrumentWithoutCommandLineOptions()
		{
			CreateUserInstrumentDefinitionFile("Mandolin", 4, "E5", "A4", "D4", "G3");
			CreateTabFile("runner-mandolin-tab", "Mandolin");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--tab", "runner-mandolin-tab" });

			Assert.Contains(harness.Console.Messages, message => message.Contains("Using string instrument: Mandolin"));
			Assert.NotNull(harness.Factory.CreatedSequence);
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_WhenTabReferencesUnknownStringInstrument_WritesError()
		{
			CreateTabFile("runner-missing-instrument-tab", "Not A Real Instrument");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--tab", "runner-missing-instrument-tab" });

			Assert.Contains(harness.Console.Messages, message => message.Contains("Unsupported string instrument: Not A Real Instrument"));
		}

		[Fact]
		public void Run_WhenExportMidiIsRequested_WritesMidiFileAndKeepsTabPlayback()
		{
			CreateTabFile("runner-midi-tab", "Standard");
			var outputPath = Path.Combine(testDirectory, "runner-midi-tab.mid");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--tab", "runner-midi-tab", "--export-midi", outputPath });

			Assert.True(File.Exists(outputPath));
			Assert.Contains(harness.Console.Messages, message => message.Contains("Exported MIDI"));
			Assert.True(harness.Player.PlayCount > 0);
		}

		[Fact]
		public void Run_WhenExportMidiIsRequestedForSongFile_WritesMidiFileAndKeepsSongPlayback()
		{
			CreateSongWithoutKeyCollection("runner-midi-song");
			var outputPath = Path.Combine(testDirectory, "runner-midi-song.mid");
			var harness = CreateHarness();

			harness.Runner.Run(new[] { "--file", "runner-midi-song", "--export-midi", outputPath, "--speed", "1500" });

			Assert.True(File.Exists(outputPath));
			Assert.Contains(harness.Console.Messages, message => message.Contains("Exported MIDI"));
			Assert.True(harness.Player.PlayCount > 0);
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

		private static void CreateTabFile(string fileName, string tuning)
		{
			Directory.CreateDirectory("Tabs");
			File.WriteAllText(Path.Combine("Tabs", $"{fileName}.json"), $"{{\"name\":\"Tab\",\"speed\":1000,\"tab\":\"1-0\",\"tuning\":\"{tuning}\",\"repeat\":1,\"default\":\"Lead\",\"versions\":[{{\"name\":\"Lead\",\"speed\":0,\"tab\":\"1-0,2-1\",\"tuning\":\"{tuning}\"}}]}}");
		}

		private static void CreateGtabFile(string fileName, string tuning)
		{
			Directory.CreateDirectory("GTabs");
			var stringNotes = tuning == "Standard" ? "[\"E\",\"A\",\"D\",\"G\",\"B\",\"E\"]" : "[\"E\",\"A\",\"D\",\"G\",\"B\",\"E\"]";
			File.WriteAllText(Path.Combine("GTabs", fileName), $"{{\"cFret\":0,\"title\":\"Gtab\",\"tempo\":120,\"stringNotes\":{stringNotes},\"version\":5,\"lyricSize\":100,\"tabRows\":[{{\"lyricLines\":[],\"columnHeaders\":[],\"columns\":[[{{\"p\":\"—\",\"s\":\"\"}},{{\"p\":\"—\",\"s\":\"\"}},{{\"p\":\"2\",\"s\":\"\"}},{{\"p\":\"—\",\"s\":\"\"}},{{\"p\":\"—\",\"s\":\"\"}},{{\"p\":\"—\",\"s\":\"\"}}],[{{\"p\":\"—\",\"s\":\"\"}},{{\"p\":\"0\",\"s\":\"\"}},{{\"p\":\"—\",\"s\":\"\"}},{{\"p\":\"—\",\"s\":\"\"}},{{\"p\":\"—\",\"s\":\"\"}},{{\"p\":\"—\",\"s\":\"\"}}]],\"lyrics\":\"\"}}]}}");
		}

		private static void CreateSevenStringTabFile(string fileName)
		{
			Directory.CreateDirectory("Tabs");
			File.WriteAllText(Path.Combine("Tabs", $"{fileName}.json"), "{\"name\":\"Seven String Tab\",\"speed\":1000,\"strings\":7,\"tab\":\"7-0\",\"tuning\":\"7 String Drop A\",\"repeat\":1,\"default\":\"Lead\",\"versions\":[{\"name\":\"Lead\",\"speed\":0,\"tab\":\"7-0,1-0\",\"tuning\":\"7 String Drop A\"}]}");
		}

		private static void CreateUserInstrumentDefinitionFile(string name, int strings, params string[] notes)
		{
			Directory.CreateDirectory("Instruments");
			var path = Path.Combine("Instruments", "string-instruments.json");
			var document = new StringInstrumentDefinitionDocument
			{
				Instruments = new[]
				{
					new StringInstrumentDefinition
					{
						Name = name,
						NumberOfStrings = strings,
						Frets = 20,
						OpenStrings = notes.Select((note, index) => new StringInstrumentStringDefinition
						{
							Number = index + 1,
							Note = note
						}).ToArray()
					}
				}
			};
			File.WriteAllText(path, JsonConvert.SerializeObject(document));
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
