namespace NoteScalerTests.Models
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using NoteScalerTests.Support;
	using System.Linq;
	using Xunit;

	public class CompositeNoteTests
	{
		[Theory]
		[InlineData("C4-250", "C", 4, 250)]
		[InlineData("A4-500", "A", 4, 500)]
		[InlineData("W-125", "W", 0, 125)]
		public void Constructor_ParsesNoteFrequencyDurationValues(string noteDefinition, string expectedNote, int expectedOctave, int expectedDuration)
		{
			var compositeNote = new CompositeNote(InstrumentType.Horn, new TestPlayer(), new[] { noteDefinition });

			var actual = compositeNote.Notes.Single();

			Assert.Equal(expectedNote, actual.Note);
			Assert.Equal(expectedOctave, actual.Octave);
			Assert.Equal(expectedDuration, actual.Duration);
		}

		[Theory]
		[InlineData(InstrumentType.Horn)]
		[InlineData(InstrumentType.Flute)]
		public void Play_DelegatesToConfiguredPlayer(InstrumentType instrument)
		{
			var player = new TestPlayer();
			var compositeNote = new CompositeNote(instrument, player, new[] { "C4-250" });

			compositeNote.Play();

			Assert.Equal(1, player.PlayCount);
			Assert.Equal(instrument, player.LastInstrument);
			Assert.Single(player.LastNotes);
		}
	}
}
