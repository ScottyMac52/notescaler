namespace NoteScaler.Services
{
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;
	using System;
	using System.Linq;

	public sealed class MusicNotePlayer : IMusicNotePlayer
	{
		private readonly IMusicNoteFactory musicNoteFactory;
		private readonly IMusicNoteChordSelector chordSelector;

		public MusicNotePlayer(IMusicNoteFactory musicNoteFactory, IMusicNoteChordSelector chordSelector)
		{
			this.musicNoteFactory = musicNoteFactory ?? throw new ArgumentNullException(nameof(musicNoteFactory));
			this.chordSelector = chordSelector ?? throw new ArgumentNullException(nameof(chordSelector));
		}

		public void Play(MusicNote musicNote)
		{
			if (musicNote.DesiredOctave > musicNote.Frequencies.Count() - 1)
			{
				var error = new ArgumentException($"{musicNote.DesiredOctave} is too high of an Octave to play {musicNote.Key}", nameof(musicNote.DesiredOctave));
				musicNote.RaiseError(error);
				return;
			}

			try
			{
				var desiredChord = chordSelector.SelectChord(musicNote);
				musicNote.RaisePlayingNote();
				var musicNotes = desiredChord.Select(note => musicNoteFactory.Create(note));
				var frequencies = musicNotes.Select(note => new FrequencyDuration(note.Key, note.DesiredOctave, note.Frequencies[musicNote.DesiredOctave], musicNote.Duration));
				musicNote.NotePlayer?.Play(frequencies, musicNote.Instrument);
			}
			catch (Exception ex)
			{
				musicNote.RaiseError(ex);
			}
		}
	}
}
