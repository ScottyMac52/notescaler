namespace NoteScalerTests
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;
	using static NoteScaler.Services.PlayEngineBase;

	public class MusicNotePlayerTests
	{
		[Fact]
		public void Play_PlaysSelectedChordFrequencies()
		{
			var factory = CreateFactory();
			var note = factory.Create("C4", player: new TestPlayer(), duration: 250, currentInstrument: InstrumentType.Flute, chordType: ChordType.Power);
			var player = (TestPlayer)note.NotePlayer;
			var service = new MusicNotePlayer(factory, new MusicNoteChordSelector());

			service.Play(note);

			Assert.Equal(InstrumentType.Flute, player.Instrument);
			Assert.Equal(new[] { "C", "G" }, player.PlayedNotes.Select(note => note.Note).ToArray());
			Assert.All(player.PlayedNotes, note => Assert.Equal(250, note.Duration));
		}

		[Fact]
		public void Play_WhenOctaveIsTooHigh_RaisesErrorAndDoesNotCallPlayer()
		{
			var factory = CreateFactory();
			var note = factory.Create("C12", player: new TestPlayer());
			var player = (TestPlayer)note.NotePlayer;
			var service = new MusicNotePlayer(factory, new MusicNoteChordSelector());
			Exception error = null;
			note.Error += (_, ex) => error = ex;

			service.Play(note);

			Assert.NotNull(error);
			Assert.Contains("too high", error.Message);
			Assert.Empty(player.PlayedNotes);
		}

		private static MusicNoteFactory CreateFactory()
		{
			return new MusicNoteFactory(new MusicNoteCache(), new MusicNoteScaleBuilder(), new MusicNoteFrequencyCalculator());
		}

		private sealed class TestPlayer : IPlayer
		{
			public IEnumerable<FrequencyDuration> PlayedNotes { get; private set; } = Array.Empty<FrequencyDuration>();
			public InstrumentType Instrument { get; private set; }
			public bool CanPause => false;
			public bool CanStop => false;
			public event PlayerEventHandler PlayerEvent
			{
				add { }
				remove { }
			}

			public void Play(IEnumerable<FrequencyDuration> noteList, InstrumentType instrument)
			{
				PlayedNotes = noteList.ToArray();
				Instrument = instrument;
			}

			public void Stop()
			{
			}

			public void Pause()
			{
			}
		}
	}
}
