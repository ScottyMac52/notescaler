namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using Xunit;

	public class MusicNoteChordTests
	{
		[Theory]
		[InlineData("C", "C0,G0", "C0,D#0,G0", "C0,E0,G0")]
		[InlineData("D", "D0,A0", "D0,F0,A0", "D0,F#0,A0")]
		public void Create_BuildsChordShapesFromScales(string note, string expectedPowerChordCsv, string expectedMinorThirdCsv, string expectedMajorThirdCsv)
		{
			var actual = MusicNote.Create(note);

			Assert.Equal(expectedPowerChordCsv.Split(','), actual.PowerChord);
			Assert.Equal(expectedMinorThirdCsv.Split(','), actual.MinorChord3);
			Assert.Equal(expectedMajorThirdCsv.Split(','), actual.MajorChord3);
		}

		[Theory]
		[InlineData("C3", 250, InstrumentType.Flute, ChordType.Note, "C3-250ms")]
		[InlineData("D4", 750, InstrumentType.Clarinet, ChordType.Power, "D4-750ms")]
		public void Create_UpdatesPlayableSettingsForCachedNotes(string note, int duration, InstrumentType instrument, ChordType chordType, string expectedDisplay)
		{
			var actual = MusicNote.Create(note, duration: duration, currentInstrument: instrument, chordType: chordType);

			Assert.NotNull(actual);
			Assert.Equal(duration, actual.Duration);
			Assert.Equal(instrument, actual.Instrument);
			Assert.Equal(chordType, actual.ChordType);
			Assert.Equal(expectedDisplay, actual.ToString());
		}

		[Theory]
		[InlineData("BadNote")]
		[InlineData("Zz")]
		public void Create_ReturnsNullForInvalidNotes(string note)
		{
			var actual = MusicNote.Create(note);

			Assert.Null(actual);
		}
	}
}
