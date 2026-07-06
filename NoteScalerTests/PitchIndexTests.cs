namespace NoteScalerTests
{
	using NoteScaler.Models;
	using System;
	using System.Linq;
	using Xunit;

	public class PitchIndexTests
	{
		[Theory]
		[InlineData("C0", 0)]
		[InlineData("C#0", 1)]
		[InlineData("B0", 11)]
		[InlineData("C1", 12)]
		[InlineData("A4", 57)]
		[InlineData("B8", 107)]
		public void GetIndex_ReturnsChromaticIndexForSupportedNote(string note, int expectedIndex)
		{
			var actual = PitchIndex.Default.GetIndex(note);

			Assert.Equal(expectedIndex, actual);
		}

		[Theory]
		[InlineData("C0", true)]
		[InlineData("B8", true)]
		[InlineData("H2", false)]
		[InlineData("C9", false)]
		[InlineData(null, false)]
		[InlineData("", false)]
		[InlineData(" ", false)]
		public void Contains_ReturnsWhetherNoteIsSupported(string note, bool expectedResult)
		{
			var actual = PitchIndex.Default.Contains(note);

			Assert.Equal(expectedResult, actual);
		}

		[Theory]
		[InlineData("C3", "C4", 12)]
		[InlineData("E2", "C3", 8)]
		[InlineData("C#2", "C3", 11)]
		[InlineData("D2", "D#2", 1)]
		[InlineData("C4", "C3", -12)]
		public void GetInterval_ReturnsSemitonesBetweenNotes(string startNote, string endNote, int expectedInterval)
		{
			var actual = PitchIndex.Default.GetInterval(startNote, endNote);

			Assert.Equal(expectedInterval, actual);
		}

		[Fact]
		public void GetNoteAbove_ReturnsNoteAtRequestedSemitoneOffset()
		{
			var actual = PitchIndex.Default.GetNoteAbove("B0", 1);

			Assert.Equal("C1", actual);
		}

		[Theory]
		[InlineData("C0", -1)]
		[InlineData("B8", 1)]
		public void GetNoteAbove_WhenOffsetLeavesSupportedRange_ThrowsOutOfRangeException(string note, int semitones)
		{
			var exception = Assert.Throws<ArgumentOutOfRangeException>(() => PitchIndex.Default.GetNoteAbove(note, semitones));

			Assert.Contains(note, exception.Message);
		}

		[Fact]
		public void GetNotesStartingAt_ReturnsRequestedChromaticSequence()
		{
			var actual = PitchIndex.Default.GetNotesStartingAt("E2", 5).ToArray();

			Assert.Equal(new[] { "E2", "F2", "F#2", "G2", "G#2" }, actual);
		}

		[Fact]
		public void GetNotesStartingAt_WhenCountIsNegative_ThrowsOutOfRangeException()
		{
			var exception = Assert.Throws<ArgumentOutOfRangeException>(() => PitchIndex.Default.GetNotesStartingAt("E2", -1).ToArray());

			Assert.Contains("Count cannot be negative.", exception.Message);
		}

		[Fact]
		public void Notes_ExposesSupportedRangeWithoutMutableArray()
		{
			var notes = PitchIndex.Default.Notes;

			Assert.Equal(108, notes.Count);
			Assert.Equal("C0", notes[0]);
			Assert.Equal("B8", notes[^1]);
			Assert.False(notes.GetType().IsArray);
		}

		[Theory]
		[InlineData("Nope")]
		[InlineData("C9")]
		public void GetIndex_ThrowsControlledExceptionForUnsupportedNote(string note)
		{
			var exception = Assert.Throws<ArgumentException>(() => PitchIndex.Default.GetIndex(note));

			Assert.Contains(note, exception.Message);
		}
	}
}
