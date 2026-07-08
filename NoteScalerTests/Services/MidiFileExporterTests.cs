namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using NoteScaler.Services;
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
			Assert.Contains(new byte[] { 0x90, 60, 87 }, bytes);
			Assert.Contains(new byte[] { 0x80, 60, 0 }, bytes);
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
			Assert.Contains(new byte[] { 0x90, 61, 100 }, bytes);
			Assert.Contains(new byte[] { 0x80, 61, 0 }, bytes);
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
			Assert.Contains(new byte[] { 0x00, 0x90, 64, 100, 0x00, 0x90, 60, 100, 0x00, 0x90, 55, 100 }, bytes);
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
	}
}
