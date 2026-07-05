namespace NoteScalerTests.Services
{
	using NoteScaler.Services;
	using Xunit;

	public class GuitarStringAdditionalTests
	{
		[Theory]
		[InlineData("E2", 0)]
		[InlineData("F2", 1)]
		[InlineData("NotANote", GuitarString.NOTE_NOT_FOUND)]
		public void GetNote_ReturnsFretOrNotFoundForRequestedNote(string note, int expectedFret)
		{
			var guitarString = new GuitarString(6, "E2", 21);

			var actual = guitarString.GetNote(note);

			Assert.Equal(expectedFret, actual);
		}

		[Theory]
		[InlineData(0, "E2")]
		[InlineData(1, "F2")]
		[InlineData(99, null)]
		public void GetNote_ReturnsNoteOrNullForRequestedFret(int fret, string expectedNote)
		{
			var guitarString = new GuitarString(6, "E2", 21);

			var actual = guitarString.GetNote(fret);

			Assert.Equal(expectedNote, actual);
		}
	}
}
