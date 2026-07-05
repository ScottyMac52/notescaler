namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using NoteScalerTests.Support;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	public class PlayableSequencePlaybackTests
	{
		[Fact]
		public void Prepare_CreatesPlayerGuitarAndCompositeNotes()
		{
			var player = new TestPlayer();
			var playableSequence = CreateSequence(player);
			playableSequence.ConvertSongNotesToNoteSequence(new SongKey("Test", "C,E,G"));

			playableSequence.Prepare();

			Assert.Same(player, playableSequence.NotePlayer);
			Assert.IsType<Guitar>(playableSequence.Guitar);
			Assert.Equal(3, playableSequence.CompositeNotes.Count());
		}

		[Theory]
		[InlineData(1, 2)]
		[InlineData(2, 4)]
		public void Play_RepeatsPreparedCompositeNotes(int repeat, int expectedPlayCount)
		{
			var player = new TestPlayer();
			var playableSequence = CreateSequence(player);
			playableSequence.Repeat = repeat;
			playableSequence.ConvertSongNotesToNoteSequence(new SongKey("Test", "C,E"));
			playableSequence.Prepare();

			playableSequence.Play();

			Assert.Equal(expectedPlayCount, player.PlayCount);
		}

		[Fact]
		public void Play_RaisesStartAndStopEventsForPreparedSequence()
		{
			var player = new TestPlayer();
			var playableSequence = CreateSequence(player);
			var events = new List<PlayableEventType>();
			playableSequence.PlayableSequenceEvent += (_, e) => events.Add(e.EventType);
			playableSequence.ConvertSongNotesToNoteSequence(new SongKey("Test", "C"));
			playableSequence.Prepare();

			playableSequence.Play();

			Assert.Contains(PlayableEventType.StartSequence, events);
			Assert.Contains(PlayableEventType.StopSequence, events);
		}

		[Fact]
		public void Play_RaisesErrorWhenThereAreNoCompositeNotes()
		{
			var playableSequence = CreateSequence(new TestPlayer());
			PlayableSequenceEvent captured = null;
			playableSequence.PlayableSequenceEvent += (_, e) => captured = e;

			playableSequence.Play();

			Assert.NotNull(captured);
			Assert.Equal(PlayableEventType.Error, captured.EventType);
			Assert.Equal("There are no notes to play", captured.EventDetails);
		}

		private static PlayableSequence CreateSequence(TestPlayer player)
		{
			return new PlayableSequence(() => player, () => new Guitar())
			{
				MeasureTime = 1000,
				Octave = 3,
				InstrumentType = InstrumentType.Horn,
				A4Reference = 440
			};
		}
	}
}
