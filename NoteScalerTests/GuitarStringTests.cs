namespace NoteScalerTests
{
	using NoteScaler;
	using NoteScaler.Classes;
	using System;
	using Xunit;

	public class GuitarStringTests
	{
		[Fact]
		public void GuiatString_EnsureStandardTuneEForString6IsCorrect()
		{
			// ARRANGE
			var stringNumber = 6;
			var tuning = "E";
			var maxFrets = 21;
			var expectedCNoteIndex = 8;
			var startingOctave = 3;

			// ACT
			var actual = new GuitarString(stringNumber, tuning, maxFrets, startingOctave);
			var cNoteIndex = actual.NoteToFret["C4"];

			// ASSERT
			Assert.Equal(tuning, actual.Tuning);
			Assert.Equal(expectedCNoteIndex, cNoteIndex);
		}

		[Fact]
		public void GuiatString_EnsureDropCSharpForString6IsCorrect()
		{
			// ARRANGE
			var stringNumber = 6;
			var tuning = "C#";
			var maxFrets = 12;
			var expectedCNoteIndex = 11;
			var startingOctave = 3;

			// ACT
			var actual = new GuitarString(stringNumber, tuning, maxFrets, startingOctave);
			var cNoteIndex = actual.NoteToFret["C4"];

			// ASSERT
			Assert.Equal(tuning, actual.Tuning);
			Assert.Equal(expectedCNoteIndex, cNoteIndex);
		}

		[Fact]
		public void GuiatString_DropDTuningForString6IsCorrect()
		{
			// ARRANGE
			var stringNumber = 6;
			var tuning = "D";
			var maxFrets = 24;
			var expectedCNoteIndex = 1;
			var startingOctave = 3;

			// ACT
			var actual = new GuitarString(stringNumber, tuning, maxFrets, startingOctave);
			var cNoteIndex = actual.NoteToFret["D#3"];

			// ASSERT
			Assert.Equal(tuning, actual.Tuning);
			Assert.Equal(expectedCNoteIndex, cNoteIndex);
		}


	}
}
