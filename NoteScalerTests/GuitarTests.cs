namespace NoteScalerTests
{
	using NoteScaler;
	using NoteScaler.Classes;
	using Xunit;

	public class GuitarTests
	{

		[Fact]
		public void GuitarTest_EnsureStandardTuningIsCorrect()
		{
			// ARRANGE
			var expectedDNote = "D";
			var expectedENote = "E"; 
			var expectedFNote = "F";

			// ACT
			var guitar = new Guitar();

			var dNote = guitar.GetNote(5, 5);
			var eNote = guitar.GetNote(5, 7);
			var fNote = guitar.GetNote(5, 8);

			// ASSERT
			Assert.Contains(expectedDNote, dNote);
			Assert.Contains(expectedENote, eNote);
			Assert.Contains(expectedFNote, fNote);

		}

		[Fact]
		public void GuitarTest_EnsureGABLocationsAreCorrectAllStrings()
		{
			// ARRANGE
			var expectedgNotePos = 3;
			var expectedaNotePos = 5;
			var expectedbNotePos = 7;
			var expectedgNotePos2 = 15;
			var expectedaNotePos2 = 17;
			var expectedbNotePos2 = 19;
			var expectedGNote1BString = 8;
			var expectedANote1BString = 10;
			var expectedBNote1BString = 12;
			var expectedGNote2BString = 20;
			var expectedANote2BString = 22;
			var expectedGNote3String = 0;
			var expectedANote3String = 2;
			var expectedBNote3String = 4;
			var expectedGNote3String1 = 12;
			var expectedANote3String1 = 14;
			var expectedBNote3String1 = 16;

			var expectedGNote4String = 5;
			var expectedANote4String = 7;
			var expectedBNote4String = 9;
			var expectedGNote4String1 = 17;
			var expectedANote4String1 = 19;
			var expectedBNote4String1 = 21;

			var expectedGNote5String = 10;
			var expectedANote5String = 12;
			var expectedBNote5String = 14;
			var expectedGNote5String1 = 22;

			var lastFret = 24;


			// ACT
			var guitar = new Guitar();

			var gNotePos3 = guitar.GetNote(1, "G4");
			var aNotePos3 = guitar.GetNote(1, "A4");
			var bNotePos3 = guitar.GetNote(1, "B4");
			var gNotePos4 = guitar.GetNote(1, "G5");
			var aNotePos4 = guitar.GetNote(1, "A5");
			var bNotePos4 = guitar.GetNote(1, "B5");

			var gNotePos5 = guitar.GetNote(2, "G5");
			var aNotePos5 = guitar.GetNote(2, "A5");
			var bNotePos5 = guitar.GetNote(2, "B5");
			var gNotePos51 = guitar.GetNote(2, "G6");
			var aNotePos51 = guitar.GetNote(2, "A6");
			var bNotePos51 = guitar.GetNote(2, "B6");

			var gNoteGPos = guitar.GetNote(3, "G3");
			var aNoteGPos = guitar.GetNote(3, "A3");
			var bNoteGPos = guitar.GetNote(3, "B3");
			var gNoteGPos1 = guitar.GetNote(3, "G4");
			var aNoteGPos1 = guitar.GetNote(3, "A4");
			var bNoteGPos1 = guitar.GetNote(3, "B4");
			var bNoteGPos2 = guitar.GetNote(3, "G5");

			var gNoteDPos = guitar.GetNote(4, "G3");
			var aNoteDPos = guitar.GetNote(4, "A3");
			var bNoteDPos = guitar.GetNote(4, "B3");
			var gNoteDPos1 = guitar.GetNote(4, "G4");
			var aNoteDPos1 = guitar.GetNote(4, "A4");
			var bNoteDPos1 = guitar.GetNote(4, "B4");

			var gNoteAPos = guitar.GetNote(5, "G4");
			var aNoteAPos = guitar.GetNote(5, "A4");
			var bNoteAPos = guitar.GetNote(5, "B4");
			var gNoteAPos1 = guitar.GetNote(5, "G5");
			var aNoteAPos1 = guitar.GetNote(5, "A5");

			var gNotePos = guitar.GetNote(6, "G2");
			var aNotePos = guitar.GetNote(6, "A2");
			var bNotePos = guitar.GetNote(6, "B2");
			var gNotePos2 = guitar.GetNote(6, "G3");
			var aNotePos2 = guitar.GetNote(6, "A3");
			var bNotePos2 = guitar.GetNote(6, "B3");

			// ASSERT
			Assert.Equal(expectedgNotePos, gNotePos);
			Assert.Equal(expectedaNotePos, aNotePos);
			Assert.Equal(expectedbNotePos, bNotePos);
			Assert.Equal(expectedgNotePos2, gNotePos2);
			Assert.Equal(expectedaNotePos2, aNotePos2);
			Assert.Equal(expectedbNotePos2, bNotePos2);
			Assert.Equal(expectedgNotePos, gNotePos3);
			Assert.Equal(expectedaNotePos, aNotePos3);
			Assert.Equal(expectedbNotePos, bNotePos3);
			Assert.Equal(expectedgNotePos2, gNotePos4);
			Assert.Equal(expectedaNotePos2, aNotePos4);
			Assert.Equal(expectedbNotePos2, bNotePos4);

			Assert.Equal(expectedGNote1BString, gNotePos5);
			Assert.Equal(expectedANote1BString, aNotePos5);
			Assert.Equal(expectedBNote1BString, bNotePos5);
			Assert.Equal(expectedGNote2BString, gNotePos51);
			Assert.Equal(expectedANote2BString, aNotePos51);
			Assert.Equal(lastFret, bNotePos51);

			Assert.Equal(expectedGNote3String, gNoteGPos);
			Assert.Equal(expectedANote3String, aNoteGPos);
			Assert.Equal(expectedBNote3String, bNoteGPos);

			Assert.Equal(expectedGNote3String1, gNoteGPos1);
			Assert.Equal(expectedANote3String1, aNoteGPos1);
			Assert.Equal(expectedBNote3String1, bNoteGPos1);
			Assert.Equal(lastFret, bNoteGPos2);

			Assert.Equal(expectedGNote4String, gNoteDPos);
			Assert.Equal(expectedANote4String, aNoteDPos);
			Assert.Equal(expectedBNote4String, bNoteDPos);
			Assert.Equal(expectedGNote4String1, gNoteDPos1);
			Assert.Equal(expectedANote4String1, aNoteDPos1);
			Assert.Equal(expectedBNote4String1, bNoteDPos1);

			Assert.Equal(expectedGNote5String, gNoteAPos);
			Assert.Equal(expectedANote5String, aNoteAPos);
			Assert.Equal(expectedBNote5String, bNoteAPos);
			Assert.Equal(expectedGNote5String1, gNoteAPos1);
			Assert.Equal(lastFret, aNoteAPos1);
		}


	}
}
