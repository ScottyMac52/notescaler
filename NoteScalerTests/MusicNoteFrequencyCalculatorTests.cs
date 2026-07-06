namespace NoteScalerTests
{
	using NoteScaler.Services;
	using System;
	using Xunit;

	public class MusicNoteFrequencyCalculatorTests
	{
		[Fact]
		public void CalculateFrequencies_ForAReferenceNote_ReturnsOctaveFrequencies()
		{
			var calculator = new MusicNoteFrequencyCalculator();

			var actual = calculator.CalculateFrequencies(9, 440);

			Assert.Equal(12, actual.Length);
			Assert.Equal(220, actual[3], 3);
			Assert.Equal(440, actual[4], 3);
			Assert.Equal(880, actual[5], 3);
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(13)]
		public void CalculateFrequencies_WhenNoteIndexIsOutOfRange_ThrowsArgumentOutOfRangeException(int noteIndex)
		{
			var calculator = new MusicNoteFrequencyCalculator();

			Assert.Throws<ArgumentOutOfRangeException>(() => calculator.CalculateFrequencies(noteIndex, 440));
		}
	}
}
