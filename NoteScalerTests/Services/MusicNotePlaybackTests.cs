namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using NoteScalerTests.Support;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;
	using static NoteScaler.Services.PlayEngineBase;

	public class MusicNotePlaybackTests
	{
		[Theory]
		[InlineData(ChordType.Note, 1)]
		[InlineData(ChordType.Power, 2)]
		[InlineData(ChordType.MinorThird, 3)]
		[InlineData(ChordType.MajorThird, 3)]
		[InlineData(ChordType.MinorSeventh, 5)]
		[InlineData(ChordType.MajorSeventh, 5)]
		public void PlayNote_PlaysRequestedChordShape(ChordType chordType, int expectedNotes)
		{
			var player = new TestPlayer();
			var musicNote = MusicNote.Create("C4", player: player, duration: 125, currentInstrument: InstrumentType.Flute, chordType: chordType);
			var playingNoteRaised = false;
			musicNote.PlayingNote += (_, _) => playingNoteRaised = true;

			musicNote.PlayNote();

			Assert.True(playingNoteRaised);
			Assert.Equal(1, player.PlayCount);
			Assert.Equal(InstrumentType.Flute, player.LastInstrument);
			Assert.Equal(expectedNotes, player.LastNotes.Count());
			Assert.All(player.LastNotes, note => Assert.Equal(125, note.Duration));
		}

		[Fact]
		public void PlayNote_RaisesErrorWhenDesiredOctaveIsTooHigh()
		{
			var musicNote = MusicNote.Create("C12", player: new TestPlayer());
			Exception captured = null;
			musicNote.Error += (_, exception) => captured = exception;

			musicNote.PlayNote();

			Assert.NotNull(captured);
			Assert.Contains("too high of an Octave", captured.Message);
		}

		[Fact]
		public void PlayNote_RaisesErrorWhenPlayerThrows()
		{
			var musicNote = MusicNote.Create("D4", player: new ThrowingPlayer());
			Exception captured = null;
			musicNote.Error += (_, exception) => captured = exception;

			musicNote.PlayNote();

			Assert.NotNull(captured);
			Assert.Contains("Player failed", captured.Message);
		}

		private sealed class ThrowingPlayer : IPlayer
		{
			public bool CanPause => false;
			public bool CanStop => false;
			public event PlayerEventHandler PlayerEvent;

			public void Pause()
			{
			}

			public void Play(IEnumerable<FrequencyDuration> noteList, InstrumentType instrument)
			{
				throw new InvalidOperationException("Player failed");
			}

			public void Stop()
			{
			}
		}
	}
}
