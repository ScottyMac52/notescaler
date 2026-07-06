namespace NoteScalerTests
{
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using System.Collections.Generic;
	using Xunit;

	public class MusicNoteChordSelectorTests
	{
		public static IEnumerable<object[]> ChordCases => new[]
		{
			new object[] { ChordType.Note, new[] { "C" } },
			new object[] { ChordType.Power, new[] { "C0", "G0" } },
			new object[] { ChordType.MinorThird, new[] { "C0", "D0", "D#0" } },
			new object[] { ChordType.MajorThird, new[] { "C0", "E0", "G0" } },
			new object[] { ChordType.MinorSeventh, new[] { "C0", "D0", "D#0", "F0", "G0" } },
			new object[] { ChordType.MajorSeventh, new[] { "C0", "E0", "G0", "B0", "D0" } }
		};

		[Theory]
		[MemberData(nameof(ChordCases))]
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
