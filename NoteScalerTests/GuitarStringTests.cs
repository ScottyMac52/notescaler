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
			var tuning = "E2";
			var maxFrets = 21;
			var expectedCNoteIndex = 8;

			// ACT
			var actual = new GuitarString(stringNumber, tuning, maxFrets);
			var cNoteIndex = actual.GetNote("C3");

			// ASSERT
			Assert.Equal(tuning, actual.Tuning);
			Assert.Equal(expectedCNoteIndex, cNoteIndex);
		}

		[Fact]
		public void GuiatString_EnsureDropCSharpForString6IsCorrect()
		{
			// ARRANGE
			var stringNumber = 6;
			var tuning = "C#2";
			var maxFrets = 12;
			var expectedCNoteIndex = 11;
	
			// ACT
			var actual = new GuitarString(stringNumber, tuning, maxFrets);
			var cNoteIndex = actual.GetNote("C3");

			// ASSERT
			Assert.Equal(tuning, actual.Tuning);
			Assert.Equal(expectedCNoteIndex, cNoteIndex);
		}

		[Fact]
		public void GuiatString_DropDTuningForString6IsCorrect()
		{
			// ARRANGE
			var stringNumber = 6;
			var tuning = "D2";
			var maxFrets = 24;
			var expectedCNoteIndex = 1;

			// ACT
			var actual = new GuitarString(stringNumber, tuning, maxFrets);
			var cNoteIndex = actual.GetNote("D#2");

			// ASSERT
			Assert.Equal(tuning, actual.Tuning);
			Assert.Equal(expectedCNoteIndex, cNoteIndex);
		}


	}
}
