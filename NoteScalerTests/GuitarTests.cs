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
	}
}
