namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using NoteScalerTests.Support;
	using System;
	using System.IO;
	using Xunit;

	public sealed class MidiFileExporterTests : IDisposable
	{
		private readonly string testDirectory;

		public MidiFileExporterTests()
		{
			testDirectory = Path.Combine(Path.GetTempPath(), $"MidiFileExporterTests_{Guid.NewGuid():N}");
			Directory.CreateDirectory(testDirectory);
		}

		[Fact]
		public void Export_WritesStandardMidiHeaderAndTrackChunk()
		{
			var outputPath = Path.Combine(testDirectory, "single-note.mid");
			var exporter = new MidiFileExporter();
			var performanceEvents = new[]
			{
				new GuitarPerformanceEvent("C4", 1, 0, 0, 500)
			};

			exporter.Export(performanceEvents, outputPath);

			var bytes = File.ReadAllBytes(outputPath);
			Assert.Equal("MThd", ReadAscii(bytes, 0, 4));
			Assert.Equal("MTrk", ReadAscii(bytes, 14, 4));
		}

		[Fact]
		public void Export_WritesGuitarProgramChangeBeforeNoteEvents()
		{
			var outputPath = Path.Combine(testDirectory, "guitar-program.mid");
			var exporter = new MidiFileExporter();
			var performanceEvents = new[]
			{
				new GuitarPerformanceEvent("C4", 1, 0, 0, 500)
			};

			exporter.Export(performanceEvents, outputPath);

			var bytes = File.ReadAllBytes(outputPath);
			var programChangeIndex = IndexOf(bytes, new byte[] { 0x00, 0xC0, 24 });
			var noteOnIndex = IndexOf(bytes, new byte[] { 0x00, 0x90, 60, 100 });
			Assert.True(programChangeIndex >= 0, "Expected default guitar program change was not found.");
			Assert.True(noteOnIndex >= 0, "Expected first note-on event was not found.");
			Assert.True(programChangeIndex < noteOnIndex, "Expected program change to be written before note events.");
		}

		[Fact]
		public void Export_WhenSingleEventExists_WritesMatchingNoteOnAndNoteOffEvents()
		{
			var outputPath = Path.Combine(testDirectory, "single-note.mid");
			var exporter = new MidiFileExporter();
			var performanceEvents = new[]
			{
				new GuitarPerformanceEvent("C4", 1, 0, 0, 500, 87, GuitarArticulation.Pick)
			};

			exporter.Export(performanceEvents, outputPath);

			var bytes = File.ReadAllBytes(outputPath);
			AssertContains(bytes, new byte[] { 0x90, 60, 87 });
			AssertContains(bytes, new byte[] { 0x80, 60, 0 });
		}

		[Fact]
		public void Export_WhenSharpNoteExists_WritesCorrectMidiNoteNumber()
		{
			var outputPath = Path.Combine(testDirectory, "sharp-note.mid");
			var exporter = new MidiFileExporter();
			var performanceEvents = new[]
			{
				new GuitarPerformanceEvent("C#4", 1, 0, 0, 500)
			};

			exporter.Export(performanceEvents, outputPath);

			var bytes = File.ReadAllBytes(outputPath);
			AssertContains(bytes, new byte[] { 0x90, 61, 100 });
			AssertContains(bytes, new byte[] { 0x80, 61, 0 });
		}

		[Fact]
		public void Export_WhenChordEventsHaveSameStartOffset_WritesNoteOnsAtSameTick()
		{
			var outputPath = Path.Combine(testDirectory, "chord.mid");
			var exporter = new MidiFileExporter();
			var performanceEvents = new[]
			{
				new GuitarPerformanceEvent("E4", 1, 0, 0, 500),
				new GuitarPerformanceEvent("C4", 2, 1, 0, 500),
				new GuitarPerformanceEvent("G3", 3, 0, 0, 500)
			};

			exporter.Export(performanceEvents, outputPath);

			var bytes = File.ReadAllBytes(outputPath);
			AssertContains(bytes, new byte[] { 0x00, 0x90, 64, 100, 0x00, 0x90, 60, 100, 0x00, 0x90, 55, 100 });
		}

		[Fact]
		public void ExportCompositeNotes_WritesSongSequenceNoteEvents()
		{
			var outputPath = Path.Combine(testDirectory, "song-sequence.mid");
			var exporter = new MidiFileExporter();
			var player = new TestPlayer();
			var compositeNotes = new[]
			{
				new CompositeNote(InstrumentType.Horn, player, new[] { "C4-500" }),
				new CompositeNote(InstrumentType.Horn, player, new[] { "D4-500" })
			};

			exporter.ExportCompositeNotes(compositeNotes, outputPath);

			var bytes = File.ReadAllBytes(outputPath);
			AssertContains(bytes, new byte[] { 0x00, 0x90, 60, 100 });
			AssertContains(bytes, new byte[] { 0x83, 0x74, 0x80, 60, 0 });
			AssertContains(bytes, new byte[] { 0x00, 0x90, 62, 100 });
		}

		[Fact]
		public void ExportCompositeNotes_WhenChordIsPresent_WritesChordNotesAtSameTick()
		{
			var outputPath = Path.Combine(testDirectory, "song-chord.mid");
			var exporter = new MidiFileExporter();
			var player = new TestPlayer();
			var compositeNotes = new[]
			{
				new CompositeNote(InstrumentType.Horn, player, new[] { "C4-500", "E4-500", "G4-500" })
			};

			exporter.ExportCompositeNotes(compositeNotes, outputPath);

			var bytes = File.ReadAllBytes(outputPath);
			AssertContains(bytes, new byte[] { 0x00, 0x90, 60, 100, 0x00, 0x90, 64, 100, 0x00, 0x90, 67, 100 });
		}

		[Fact]
		public void Export_WhenOutputDirectoryDoesNotExist_CreatesDirectoryAndFile()
		{
			var outputPath = Path.Combine(testDirectory, "nested", "export.mid");
			var exporter = new MidiFileExporter();
			var performanceEvents = new[]
			{
				new GuitarPerformanceEvent("C4", 1, 0, 0, 500)
			};

			exporter.Export(performanceEvents, outputPath);

			Assert.True(File.Exists(outputPath));
		}

		public void Dispose()
		{
			if (Directory.Exists(testDirectory))
			{
				Directory.Delete(testDirectory, true);
			}
		}

		private static string ReadAscii(byte[] bytes, int startIndex, int length)
		{
			return System.Text.Encoding.ASCII.GetString(bytes, startIndex, length);
		}

		private static void AssertContains(byte[] bytes, byte[] expectedSequence)
		{
			Assert.True(ContainsSequence(bytes, expectedSequence), $"Expected byte sequence was not found: {BitConverter.ToString(expectedSequence)}");
		}

		private static bool ContainsSequence(byte[] bytes, byte[] expectedSequence)
		{
			return IndexOf(bytes, expectedSequence) >= 0;
		}

		private static int IndexOf(byte[] bytes, byte[] expectedSequence)
		{
			for (var index = 0; index <= bytes.Length - expectedSequence.Length; index++)
			{
				var found = true;
				for (var sequenceIndex = 0; sequenceIndex < expectedSequence.Length; sequenceIndex++)
				{
					if (bytes[index + sequenceIndex] != expectedSequence[sequenceIndex])
					{
						found = false;
						break;
					}
				}

				if (found)
				{
					return index;
				}
			}

			return -1;
		}
	}
}
