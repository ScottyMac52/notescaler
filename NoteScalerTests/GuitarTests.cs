namespace NoteScalerTests
{
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	public class GuitarTests
	{
		private const int OPEN_STRING = 0;
		private List<Exception> exceptionList = null;

		[Fact]
		public void GuitarTest_EnsureStandardTuningIsCorrect()
		{
			var guitar = new Guitar();

			Assert.Equal("D3", guitar.GetNote(5, 5));
			Assert.Equal("E3", guitar.GetNote(5, 7));
			Assert.Equal("F3", guitar.GetNote(5, 8));
		}

		[Fact]
		public void Guitar_CreatesExpectedOpenStringsForSupportedTunings()
		{
			var cases = new[]
			{
				new { Tuning = TuningScheme.Standard, Notes = new[] { "E4", "B3", "G3", "D3", "A2", "E2" } },
				new { Tuning = TuningScheme.DropC, Notes = new[] { "D4", "A3", "F3", "C3", "G2", "C2" } },
				new { Tuning = TuningScheme.DropCSharp, Notes = new[] { "D#4", "A#3", "F#3", "C#3", "G#2", "C#2" } },
				new { Tuning = TuningScheme.DropD, Notes = new[] { "D4", "B3", "G3", "D3", "A2", "D2" } },
				new { Tuning = TuningScheme.OpenC, Notes = new[] { "E4", "C3", "G3", "C3", "G2", "C2" } },
				new { Tuning = TuningScheme.OpenD, Notes = new[] { "D4", "A3", "F#3", "D3", "A2", "D2" } }
			};

			foreach (var currentCase in cases)
			{
				var guitar = new Guitar(currentCase.Tuning);
				var actualOpenStrings = guitar.Strings.OrderBy(currentString => currentString.Number).Select(currentString => currentString.Tuning).ToArray();

				Assert.Equal(currentCase.Notes, actualOpenStrings);
			}
		}

		[Theory]
		[InlineData(NoteScaler.Enums.TuningScheme.DropC, "C3", "D3", "D#3")]
		[InlineData(NoteScaler.Enums.TuningScheme.DropCSharp, "C#3", "D#3", "E3")]
		public void GuitarTest_EnsureDropTuningsAreCorrect(NoteScaler.Enums.TuningScheme tuningScheme, string expected5Note5, string expected5Note7, string expected5Note8)
		{
			var guitar = new Guitar(tuningScheme);

			Assert.Contains(expected5Note5, guitar.GetNote(5, 5));
			Assert.Contains(expected5Note7, guitar.GetNote(5, 7));
			Assert.Contains(expected5Note8, guitar.GetNote(5, 8));
		}

		[Fact]
		public void GuitarTest_EnsureOctavesAreCorrectStandardTuning()
		{
			var guitar = new Guitar();

			var string1Fret5 = guitar.GetNote(1, 5);
			var string2Fret5 = guitar.GetNote(2, 5);
			var string3Fret4 = guitar.GetNote(3, 4);
			var string4Fret5 = guitar.GetNote(4, 5);
			var string5Fret5 = guitar.GetNote(5, 5);
			var string6Fret5 = guitar.GetNote(6, 5);
			var string5Open = guitar.GetNote(5, 0);
			var string4Open = guitar.GetNote(4, 0);
			var string3Open = guitar.GetNote(3, 0);
			var string2Open = guitar.GetNote(2, 0);
			var string1Open = guitar.GetNote(1, 0);

			Assert.Equal("A4", string1Fret5);
			Assert.Equal("E4", string2Fret5);
			Assert.Equal("B3", string3Fret4);
			Assert.Equal("G3", string4Fret5);
			Assert.Equal("D3", string5Fret5);
			Assert.Equal("A2", string6Fret5);
			Assert.Equal(string1Open, string2Fret5);
			Assert.Equal(string2Open, string3Fret4);
			Assert.Equal(string3Open, string4Fret5);
			Assert.Equal(string4Open, string5Fret5);
			Assert.Equal(string5Open, string6Fret5);
		}

		[Fact]
		public void GuitarTest_EnsureGABLocationsAreCorrectAllStringsStandardTuning()
		{
			var guitar = new Guitar();
			var lastFret = 24;

			Assert.Equal(3, guitar.GetNote(1, "G4"));
			Assert.Equal(5, guitar.GetNote(1, "A4"));
			Assert.Equal(7, guitar.GetNote(1, "B4"));
			Assert.Equal(15, guitar.GetNote(1, "G5"));
			Assert.Equal(17, guitar.GetNote(1, "A5"));
			Assert.Equal(19, guitar.GetNote(1, "B5"));

			Assert.Equal(OPEN_STRING, guitar.GetNote(2, "B3"));
			Assert.Equal(8, guitar.GetNote(2, "G4"));
			Assert.Equal(10, guitar.GetNote(2, "A4"));
			Assert.Equal(12, guitar.GetNote(2, "B4"));
			Assert.Equal(20, guitar.GetNote(2, "G5"));
			Assert.Equal(22, guitar.GetNote(2, "A5"));
			Assert.Equal(lastFret, guitar.GetNote(2, "B5"));

			Assert.Equal(OPEN_STRING, guitar.GetNote(3, "G3"));
			Assert.Equal(2, guitar.GetNote(3, "A3"));
			Assert.Equal(4, guitar.GetNote(3, "B3"));
			Assert.Equal(12, guitar.GetNote(3, "G4"));
			Assert.Equal(14, guitar.GetNote(3, "A4"));
			Assert.Equal(16, guitar.GetNote(3, "B4"));
			Assert.Equal(lastFret, guitar.GetNote(3, "G5"));

			Assert.Equal(5, guitar.GetNote(4, "G3"));
			Assert.Equal(7, guitar.GetNote(4, "A3"));
			Assert.Equal(9, guitar.GetNote(4, "B3"));
			Assert.Equal(17, guitar.GetNote(4, "G4"));
			Assert.Equal(19, guitar.GetNote(4, "A4"));
			Assert.Equal(21, guitar.GetNote(4, "B4"));

			Assert.Equal(OPEN_STRING, guitar.GetNote(5, "A2"));
			Assert.Equal(2, guitar.GetNote(5, "B2"));
			Assert.Equal(10, guitar.GetNote(5, "G3"));
			Assert.Equal(12, guitar.GetNote(5, "A3"));
			Assert.Equal(14, guitar.GetNote(5, "B3"));
			Assert.Equal(22, guitar.GetNote(5, "G4"));
			Assert.Equal(lastFret, guitar.GetNote(5, "A4"));

			Assert.Equal(3, guitar.GetNote(6, "G2"));
			Assert.Equal(5, guitar.GetNote(6, "A2"));
			Assert.Equal(7, guitar.GetNote(6, "B2"));
			Assert.Equal(15, guitar.GetNote(6, "G3"));
			Assert.Equal(17, guitar.GetNote(6, "A3"));
			Assert.Equal(19, guitar.GetNote(6, "B3"));
		}

		[Theory]
		[InlineData(0, 2)]
		[InlineData(1, 3)]
		[InlineData(2, 4)]
		[InlineData(3, 5)]
		[InlineData(4, 6)]
		[InlineData(5, 7)]
		[InlineData(6, 8)]
		[InlineData(7, 9)]
		[InlineData(8, 10)]
		[InlineData(9, 11)]
		[InlineData(10, 12)]
		[InlineData(11, 13)]
		public void GuitarTest_TestPerfect5thIntervalsStandardTuning(int startFret, int endFret)
		{
			var guitar = new Guitar();

			var actualInterval = guitar.GetNoteInterval(6, startFret, 5, endFret, out exceptionList);

			Assert.Equal(7, actualInterval);
		}

		[Theory]
		[InlineData("C3", "C4", 12)]
		[InlineData("C4", "C3", -12)]
		[InlineData("E2", "C3", 8)]
		public void GetNoteInterval_ReturnsExpectedSemitones(string startNote, string endNote, int expectedInterval)
		{
			var guitar = new Guitar();

			var actualInterval = guitar.GetNoteInterval(startNote, endNote);

			Assert.Equal(expectedInterval, actualInterval);
		}

		[Theory]
		[InlineData("Nope", "C3")]
		[InlineData("C3", "Nope")]
		public void GetNoteInterval_WhenNoteIsUnsupported_ThrowsControlledException(string startNote, string endNote)
		{
			var guitar = new Guitar();

			var exception = Assert.Throws<ArgumentException>(() => guitar.GetNoteInterval(startNote, endNote));

			Assert.Contains("Unsupported note", exception.Message);
		}
	}
}
