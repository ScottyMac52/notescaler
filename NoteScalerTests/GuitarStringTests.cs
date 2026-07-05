namespace NoteScalerTests
{
	using NoteScaler.Services;
	using Xunit;

	public class GuitarStringTests
	{
		[Theory]
		[InlineData(6, "E2", 21, "C3", 8)]
		[InlineData(6, "C#2", 12, "C3", 11)]
		[InlineData(6, "D2", 24, "D#2", 1)]
		public void GuitarString_GetNote_ReturnsExpectedFretForTuning(int stringNumber, string tuning, int maxFrets, string note, int expectedFret)
		{
			var actual = new GuitarString(stringNumber, tuning, maxFrets);

			var fret = actual.GetNote(note);

			Assert.Equal(tuning, actual.Tuning);
			Assert.Equal(expectedFret, fret);
		}
	}
}
