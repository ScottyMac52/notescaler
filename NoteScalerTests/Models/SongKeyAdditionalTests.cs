namespace NoteScalerTests.Models
{
	using NoteScaler.Models;
	using Xunit;

	public class SongKeyAdditionalTests
	{
		[Fact]
		public void Constructor_AllowsNullSequence()
		{
			var songKey = new SongKey("No Sequence", null);

			Assert.Equal("No Sequence", songKey.Name);
			Assert.Null(songKey.Sequence);
			Assert.Null(songKey.SongNotes);
		}

		[Fact]
		public void SetNoteSequence_ReplacesExistingNotes()
		{
			var songKey = new SongKey("Test", "C,D");

			songKey.SetNoteSequence(new[] { "E", "F" });

			Assert.Equal(new[] { "E", "F" }, songKey.SongNotes);
		}
	}
}
