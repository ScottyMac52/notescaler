namespace NoteScaler.Models
{
	using NoteScaler.Classes;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	public class CompositeNote
	{
		public InstrumentType InstrumentType { get; protected set; }
		public IPlayer Player { get; protected set; }

		public CompositeNote(InstrumentType instrument, IPlayer player, IEnumerable<string> notes, int a4Reference = 440)
		{
			InstrumentType = instrument;
			Player = player;
			Notes = GetNoteFrequencyAndDuration(notes, a4Reference, player, instrument);
		}

		public IEnumerable<FrequencyDuration> Notes { get; set; }

		public void Play()
		{
			Player?.Play(Notes, InstrumentType);
		}

		private IEnumerable<FrequencyDuration> GetNoteFrequencyAndDuration(IEnumerable<string> notes, int a4Ref, IPlayer player, InstrumentType instrument)
		{
			var noteDuration = 0;
			foreach (var noteString in notes)
			{
				string currentNote = null;
				var noteInstrument = instrument;
				var notesParts = noteString.Split('-');
				if(notesParts.Length > 0)
				{
					currentNote = notesParts[0];
				}
				if (notesParts.Length > 1)
				{
					noteDuration = (int)(float.Parse(notesParts[1]));
				}
				if (notesParts.Length > 2)
				{
					noteInstrument = (InstrumentType)Enum.Parse(typeof(InstrumentType), notesParts[2]);
				}
				if(currentNote.Contains("W", StringComparison.InvariantCultureIgnoreCase))
				{
					yield return new FrequencyDuration("W", 0, 0F, noteDuration);
				}
				else
				{
					var musicNote = MusicNote.Create(currentNote, a4Ref, player, noteDuration, noteInstrument);
					yield return new FrequencyDuration(musicNote.Key, musicNote.DesiredOctave, musicNote.CurrentFrequency, noteDuration);
				}
			}
		}

	}
}
