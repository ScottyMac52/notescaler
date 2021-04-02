namespace NoteScalerTests
{
	using NoteScaler.Classes;
	using System;
	using System.Collections.Generic;
	using Xunit;

	public class GuitarTests
	{
		private const int OPEN_STRING = 0;
		private List<Exception> exceptionList = null;

		[Fact]
		public void GuitarTest_EnsureStandardTuningIsCorrect()
		{
			// ARRANGE
			var expectedDNote = "D3";
			var expectedENote = "E3"; 
			var expectedFNote = "F3";

			// ACT
			var guitar = new Guitar();

			var dNote = guitar.GetNote(5, 5);
			var eNote = guitar.GetNote(5, 7);
			var fNote = guitar.GetNote(5, 8);

			// ASSERT
			Assert.Equal(expectedDNote, dNote);
			Assert.Equal(expectedENote, eNote);
			Assert.Equal(expectedFNote, fNote);

		}

		[Fact]
		public void GuitarTest_EnsureDropCIsCorrect()
		{
			// ARRANGE
			var expected5Note5 = "C3";
			var expected5Note7 = "D3";
			var expected5Note8 = "D#3";

			// ACT
			var guitar = new Guitar(NoteScaler.Enums.TuningScheme.DropC);

			var dNote = guitar.GetNote(5, 5);
			var eNote = guitar.GetNote(5, 7);
			var fNote = guitar.GetNote(5, 8);

			// ASSERT
			Assert.Contains(expected5Note5, dNote);
			Assert.Contains(expected5Note7, eNote);
			Assert.Contains(expected5Note8, fNote);
		}

		[Fact]
		public void GuitarTest_EnsureDropCSharpIsCorrect()
		{
			// ARRANGE
			var expected5Note5 = "C#3";
			var expected5Note7 = "D#3";
			var expected5Note8 = "E3";

			// ACT
			var guitar = new Guitar(NoteScaler.Enums.TuningScheme.DropCSharp);

			var dNote = guitar.GetNote(5, 5);
			var eNote = guitar.GetNote(5, 7);
			var fNote = guitar.GetNote(5, 8);

			// ASSERT
			Assert.Contains(expected5Note5, dNote);
			Assert.Contains(expected5Note7, eNote);
			Assert.Contains(expected5Note8, fNote);
		}


		[Fact]
		public void GuitarTest_EnsureOctavesAreCorrectStandardTuning()
		{
			// ARRANGE
			var expectedNote1Fret5 = "A4";
			var expectedNote2Fret5 = "E4";
			var expectedNote3Fret4 = "B3";
			var expectedNote4Fret5 = "G3";
			var expectedNote5Fret5 = "D3";
			var expectedNote6Fret5 = "A2";

			// ACT
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

			// ASSERT
			Assert.Equal(expectedNote1Fret5, string1Fret5);
			Assert.Equal(expectedNote2Fret5, string2Fret5);
			Assert.Equal(expectedNote3Fret4, string3Fret4);
			Assert.Equal(expectedNote4Fret5, string4Fret5);
			Assert.Equal(expectedNote5Fret5, string5Fret5);
			Assert.Equal(expectedNote6Fret5, string6Fret5);
			Assert.Equal(string1Open, string2Fret5);
			Assert.Equal(string2Open, string3Fret4);
			Assert.Equal(string3Open, string4Fret5);
			Assert.Equal(string4Open, string5Fret5);
			Assert.Equal(string5Open, string6Fret5);
		}

		[Fact]
		public void GuitarTest_EnsureGABLocationsAreCorrectAllStringsStandardTuning()
		{
			// ARRANGE
			// String 1 and 6 E2 and E4
			var expectedgNote1Pos1 = 3;
			var expectedaNote1Pos1 = 5;
			var expectedbNote1Pos1 = 7;
			var expectedgNote1Pos2 = 15;
			var expectedaNote1Pos2 = 17;
			var expectedbNote1Pos2 = 19;

			// String 2 B3
			var expectedbNote2Pos0 = OPEN_STRING;
			var expectedgNote2Pos1 = 8;
			var expectedaNote2Pos1 = 10;
			var expectedbNote2Pos1 = 12;
			var expectedgNote2Pos2 = 20;
			var expectedaNote2Pos2 = 22;

			// String 3 G3
			var expectedgNote3Pos0 = OPEN_STRING;
			var expectedaNote3Pos1 = 2;
			var expectedbNote3Pos1 = 4;
			var expectedgNote3Pos2 = 12;
			var expectedaNote3Pos2 = 14;
			var expectedbNote3Pos2 = 16;

			// String 4 D3
			var expectedgNote4Pos1 = 5;
			var expectedaNote4Pos1 = 7;
			var expectedbNote4Pos1 = 9;
			var expectedgNote4Pos2 = 17;
			var expectedaNote4Pos2 = 19;
			var expectedbNote4Pos2 = 21;

			// String 5 A2
			var expectedaNote5Pos0 = OPEN_STRING;
			var expectedbNote5Pos0 = 2;
			var expectedgNote5Pos1 = 10;
			var expectedaNote5Pos1 = 12;
			var expectedbNote5Pos1 = 14;
			var expectedgNote5Pos2 = 22;
			var lastFret = 24;

			// ACT
			var guitar = new Guitar();

			var gNote1Pos1 = guitar.GetNote(1, "G4");
			var aNote1Pos1 = guitar.GetNote(1, "A4");
			var bNote1Pos1 = guitar.GetNote(1, "B4");
			var gNote1Pos2 = guitar.GetNote(1, "G5");
			var aNote1Pos2 = guitar.GetNote(1, "A5");
			var bNote1Pos2 = guitar.GetNote(1, "B5");

			var bNote2Pos0 = guitar.GetNote(2, "B3");
			var gNote2Pos1 = guitar.GetNote(2, "G4");
			var aNote2Pos1 = guitar.GetNote(2, "A4");
			var bNote2Pos1 = guitar.GetNote(2, "B4");
			var gNote2Pos2 = guitar.GetNote(2, "G5");
			var aNote2Pos2 = guitar.GetNote(2, "A5");
			var bNote2Pos2 = guitar.GetNote(2, "B5");

			var gNote3Pos0 = guitar.GetNote(3, "G3");
			var aNote3Pos1 = guitar.GetNote(3, "A3");
			var bNote3Pos1 = guitar.GetNote(3, "B3");
			var gNote3Pos2 = guitar.GetNote(3, "G4");
			var aNote3Pos2 = guitar.GetNote(3, "A4");
			var bNote3Pos2 = guitar.GetNote(3, "B4");
			var gNote3Pos3 = guitar.GetNote(3, "G5");

			var gNote4Pos1 = guitar.GetNote(4, "G3");
			var aNote4Pos1 = guitar.GetNote(4, "A3");
			var bNote4Pos1 = guitar.GetNote(4, "B3");
			var gNote4Pos2 = guitar.GetNote(4, "G4");
			var aNote4Pos2 = guitar.GetNote(4, "A4");
			var bNote4Pos2 = guitar.GetNote(4, "B4");

			var aNote5Pos0 = guitar.GetNote(5, "A2");
			var bNote5Pos0 = guitar.GetNote(5, "B2");
			var gNote5Pos1 = guitar.GetNote(5, "G3");
			var aNote5Pos1 = guitar.GetNote(5, "A3");
			var bNote5Pos1 = guitar.GetNote(5, "B3");
			var gNote5Pos2 = guitar.GetNote(5, "G4");
			var aNote5Pos2 = guitar.GetNote(5, "A4");

			var gNote6Pos1 = guitar.GetNote(6, "G2");
			var aNote6Pos1 = guitar.GetNote(6, "A2");
			var bNote6Pos1 = guitar.GetNote(6, "B2");
			var gNote6Pos2 = guitar.GetNote(6, "G3");
			var aNote6Pos2 = guitar.GetNote(6, "A3");
			var bNote6Pos2 = guitar.GetNote(6, "B3");

			// ASSERT
			// String 1
			Assert.Equal(expectedgNote1Pos1, gNote1Pos1);
			Assert.Equal(expectedaNote1Pos1, aNote1Pos1);
			Assert.Equal(expectedbNote1Pos1, bNote1Pos1);
			Assert.Equal(expectedgNote1Pos2, gNote1Pos2);
			Assert.Equal(expectedaNote1Pos2, aNote1Pos2);
			Assert.Equal(expectedbNote1Pos2, bNote1Pos2);

			// String 2
			Assert.Equal(expectedbNote2Pos0, bNote2Pos0);
			Assert.Equal(expectedgNote2Pos1, gNote2Pos1);
			Assert.Equal(expectedaNote2Pos1, aNote2Pos1);
			Assert.Equal(expectedbNote2Pos1, bNote2Pos1);
			Assert.Equal(expectedgNote2Pos2, gNote2Pos2);
			Assert.Equal(expectedaNote2Pos2, aNote2Pos2);
			Assert.Equal(lastFret, bNote2Pos2);

			// String 3
			Assert.Equal(expectedgNote3Pos0, gNote3Pos0);
			Assert.Equal(expectedaNote3Pos1, aNote3Pos1);
			Assert.Equal(expectedbNote3Pos1, bNote3Pos1);
			Assert.Equal(expectedgNote3Pos2, gNote3Pos2);
			Assert.Equal(expectedaNote3Pos2, aNote3Pos2);
			Assert.Equal(expectedbNote3Pos2, bNote3Pos2);
			Assert.Equal(lastFret, gNote3Pos3);

			// String 4
			Assert.Equal(expectedgNote4Pos1, gNote4Pos1);
			Assert.Equal(expectedaNote4Pos1, aNote4Pos1);
			Assert.Equal(expectedbNote4Pos1, bNote4Pos1);
			Assert.Equal(expectedgNote4Pos2, gNote4Pos2);
			Assert.Equal(expectedaNote4Pos2, aNote4Pos2);
			Assert.Equal(expectedbNote4Pos2, bNote4Pos2);

			// String 5
			Assert.Equal(expectedaNote5Pos0, aNote5Pos0);
			Assert.Equal(expectedbNote5Pos0, bNote5Pos0);
			Assert.Equal(expectedgNote5Pos1, gNote5Pos1);
			Assert.Equal(expectedaNote5Pos1, aNote5Pos1);
			Assert.Equal(expectedbNote5Pos1, bNote5Pos1);
			Assert.Equal(expectedgNote5Pos2, gNote5Pos2);
			Assert.Equal(lastFret, aNote5Pos2);

			// String 6
			Assert.Equal(expectedgNote1Pos1, gNote6Pos1);
			Assert.Equal(expectedaNote1Pos1, aNote6Pos1);
			Assert.Equal(expectedbNote1Pos1, bNote6Pos1);
			Assert.Equal(expectedgNote1Pos2, gNote6Pos2);
			Assert.Equal(expectedaNote1Pos2, aNote6Pos2);
			Assert.Equal(expectedbNote1Pos2, bNote6Pos2);
		}

		[Fact]
		public void GuitarTest_TestPerfect5thIntervalsStandardTuning()
		{
			// ARRANGE
			var expectedInterval = 7;

			// ACT
			var guitar = new Guitar();

			// ASSERT

			// E Power Chord
			var actualInterval = guitar.GetNoteInterval(6, 0, 5, 2, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);

			// F Power Chord
			actualInterval = guitar.GetNoteInterval(6, 1, 5, 3, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);

			// F# Power Chord
			actualInterval = guitar.GetNoteInterval(6, 2, 5, 4, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);

			// G Power Chord
			actualInterval = guitar.GetNoteInterval(6, 3, 5, 5, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);

			// G# Power Chord
			actualInterval = guitar.GetNoteInterval(6, 4, 5, 6, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);

			// A Power Chord
			actualInterval = guitar.GetNoteInterval(6, 5, 5, 7, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);

			// A# Power Chord
			actualInterval = guitar.GetNoteInterval(6, 6, 5, 8, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);

			// B Power Chord
			actualInterval = guitar.GetNoteInterval(6, 7, 5, 9, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);

			// C Power Chord
			actualInterval = guitar.GetNoteInterval(6, 8, 5, 10, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);

			// C# Power Chord
			actualInterval = guitar.GetNoteInterval(6, 9, 5, 11, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);

			// D Power Chord
			actualInterval = guitar.GetNoteInterval(6, 10, 5, 12, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);

			// D# Power Chord
			actualInterval = guitar.GetNoteInterval(6, 11, 5, 13, out exceptionList);
			Assert.Equal(expectedInterval, actualInterval);
		}
	}
}
