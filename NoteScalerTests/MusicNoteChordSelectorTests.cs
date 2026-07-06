namespace NoteScalerTests
{
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using Xunit;

	public class MusicNoteChordSelectorTests
	{
		[Theory]
		[InlineData(ChordType.Note, new[] { "C" })]
		[InlineData(ChordType.Power, new[] { "C0", "G0" })]
		[InlineData(ChordType.MinorThird, new[] { "C0", "D0", "D#0" })]
		[InlineData(ChordType.MajorThird, new[] { "C0", "E0", "G0" })]
		[InlineData(ChordType.MinorSeventh, new[] { "C0", "D0", "D#0", "F0", "G0" })]
		[InlineData(ChordType.MajorSeventh, new[] { "C0", "E0", "G0", "B0", "D0" })]
		public void SelectChord_ReturnsExpectedNotesForChordType(ChordType chordType, string[] expectedNotes)
		{
			var factory = new MusicNoteFactory(new MusicNoteCache(), new MusicNoteScaleBuilder(), new MusicNoteFrequencyCalculator());
			var note = factory.Create("C4", chordType: chordType);
			var selector = new MusicNoteChordSelector();

			var actual = selector.SelectChord(note);

			Assert.Equal(expectedNotes, actual);
		}
	}
}
