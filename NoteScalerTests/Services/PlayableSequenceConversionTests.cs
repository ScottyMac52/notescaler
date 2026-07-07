namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	public class PlayableSequenceConversionTests
	{
		[Theory]
		[InlineData("C,D-0.5,E4-2", "C3-1000|D3-500|E4-2000")]
		[InlineData("C|E|G,A", "C3-1000,E3-1000,G3-1000|A3-1000")]
		public void ConvertSongNotesToNoteSequence_ConvertsSongNotationToTimedNotes(string sequence, string expectedGroups)
		{
			var playableSequence = CreateSequence();
			var song = new SongKey("Test", sequence);

			playableSequence.ConvertSongNotesToNoteSequence(song);

			AssertNoteGroups(expectedGroups, playableSequence.NoteSequence);
		}

		[Theory]
		[InlineData("C,D-0.5,E4-2", "C3-1000|D3-500|E4-2000")]
		[InlineData("C|E|G,A", "C3-1000,E3-1000,G3-1000|A3-1000")]
		public void LoadSequenceFromString_ConvertsSongNotationWhenSongHasNotBeenInitialized(string sequence, string expectedGroups)
		{
			var playableSequence = CreateSequence();

			var exception = Record.Exception(() => playableSequence.LoadSequenceFromString(sequence.Split(',')));

			Assert.Null(exception);
			AssertNoteGroups(expectedGroups, playableSequence.NoteSequence);
		}

		[Theory]
		[InlineData("1-0-0.5,2-1,6-0", "E4-500|C4-1000|E2-1000")]
		[InlineData("1-0|2-1,3-0-0.25", "E4-1000,C4-1000|G3-250")]
		public void ConvertTabsToNoteSequence_ConvertsFrettedTabsToTimedNotes(string tabString, string expectedGroups)
		{
			var playableSequence = CreateSequence();
			var tabs = new Tablature
			{
				Name = "Tab",
				TabString = tabString,
				Tuning = "Standard"
			};

			playableSequence.ConvertTabsToNoteSequence(new Guitar(), tabs);

			AssertNoteGroups(expectedGroups, playableSequence.NoteSequence);
		}

		private static PlayableSequence CreateSequence()
		{
			return new PlayableSequence
			{
				MeasureTime = 1000,
				Octave = 3,
				InstrumentType = InstrumentType.Horn,
				A4Reference = 440
			};
		}

		private static void AssertNoteGroups(string expectedGroups, IEnumerable<NoteGroup> actualGroups)
		{
			var expected = expectedGroups.Split('|').Select(group => group.Split(',')).ToArray();
			var actual = actualGroups.Select(group => group.Notes).ToArray();

			Assert.Equal(expected.Length, actual.Length);
			for (var index = 0; index < expected.Length; index++)
			{
				Assert.Equal(expected[index], actual[index]);
			}
		}
	}
}
