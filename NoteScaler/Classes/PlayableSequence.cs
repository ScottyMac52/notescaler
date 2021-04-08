namespace NoteScaler.Classes
{
	using Newtonsoft.Json;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	/// <summary>
	/// Used to create and play note sequences that are sourced from <see cref="Tablature"/> or <see cref="Song"/>
	/// </summary>
	public class PlayableSequence
	{
		#region Public properties

		public SongKey Song { get; protected set; }
		public int MeasureTime { get; set; } = 1000;
		public int Octave { get; set; } = 2;
		public InstrumentType InstrumentType { get; set; } = InstrumentType.Horn;
		public int A4Reference { get; set; } = 440;
		public IPlayer NotePlayer { get; private set; }
		public IStringInstrument Guitar { get; private set; }
		public int? Repeat { get; set; }
		public IEnumerable<NoteGroup> NoteSequence { get; set; }
		public IEnumerable<CompositeNote> CompositeNotes { get; protected set; }

		#endregion Public properties

		#region Events

		public delegate void PlayableSequenceEventHandler(object sender, PlayableSequenceEvent e);
		public event PlayableSequenceEventHandler PlayableSequenceEvent;

		#endregion Events

		#region Public methods

		public void Prepare()
		{
			NotePlayer = new SignalNotePlayer();
			NotePlayer.PlayerEvent += NotePlayer_PlayerEvent;
			Guitar = new Guitar();
			PrepareSequence();
		}

		private void NotePlayer_PlayerEvent(object sender, PlayerEngineEvent e)
		{
			if (e.EventType == PlayerEventType.PlayNotes)
			{
				PlayableSequenceEvent?.Invoke(this, new PlayableSequenceEvent() { EventType = PlayableEventType.PlayingNote, EventDetails = e.Message });
			}
		}

		/// <summary>
		/// Turns the string notes in <see cref="Song"/> into <see cref="CompositeNote"/>s
		/// </summary>
		public void PrepareSequence()
		{
			CompositeNotes = GetCompositeNotesFromSequence();
		}

		public void ReverseSequence()
		{
			NoteSequence = NoteSequence.Reverse();
		}

		/// <summary>
		/// Plays the sequence of notes in <see cref="NoteSequence"/>
		/// </summary>
		public void Play()
		{
			if ((CompositeNotes?.Count() ?? 0) == 0)
			{
				PlayableSequenceEvent?.Invoke(this, new PlayableSequenceEvent() { EventType = PlayableEventType.Error, EventDetails = "There are no notes to play" });
				return;
			}

			var repeat = Repeat ?? 1;
			var message = $"Playing sequence of {CompositeNotes?.Count() ?? 0} notes {repeat} time(s).";
			PlayableSequenceEvent?.Invoke(this, new PlayableSequenceEvent() { EventType = PlayableEventType.StartSequence, EventDetails = message });
			for (var counter = repeat; counter > 0; counter--)
			{
				foreach (var songNoteDuration in CompositeNotes)
				{
					try
					{
						NotePlayer.Play(songNoteDuration.Notes, InstrumentType);
					}
					catch (Exception ex)
					{
						PlayableSequenceEvent?.Invoke(this, new PlayableSequenceEvent() { EventType = PlayableEventType.Error, EventDetails = $"{ex}" });
					}
				}
			}
			PlayableSequenceEvent?.Invoke(this, new PlayableSequenceEvent() { EventType = PlayableEventType.StopSequence, EventDetails = $"Finished playing {CompositeNotes.Count()} notes." });
		}

		/// <summary>
		/// Loads a sequence of octave weighted, timed and instrument specific notes from a string into <see cref="NoteSequence"/> 
		/// </summary>
		/// <param name="sequence"></param>
		public void LoadSequenceFromString(IEnumerable<string> sequence)
		{
			Song.SetNoteSequence(sequence.ToArray());
			PrepareSequence();
			PlayableSequenceEvent?.Invoke(this, new PlayableSequenceEvent()
			{
				EventType = PlayableEventType.SequenceLoaded,
				EventDetails = $"{NoteSequence.Count()} Notes loaded."
			});
		}

		/// <summary>
		/// Loads a <see cref="Song"/> or <see cref="Tablature"/> Definition from a file
		/// </summary>
		/// <param name="tabName"></param>
		/// <param name="directory"></param>
		/// <param name="errorString"></param>
		/// <param name="result"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns><see cref="bool"/></returns>
		public bool LoadFromFile<T>(string fileName, string directory, out string errorString, out T result)
		{
			result = default;
			if (!string.IsNullOrEmpty(fileName))
			{
				try
				{
					var fileNameWithExt = $"{fileName}.json";
					var completeFileName = Path.Combine(Environment.CurrentDirectory, $"{directory}\\{fileNameWithExt}");
					if (!File.Exists(completeFileName))
					{
						errorString = new FileNotFoundException($"File not found in {Path.Combine(Environment.CurrentDirectory, directory)}", fileNameWithExt).ToString();
						return false;
					}
					result = JsonConvert.DeserializeObject<T>(File.ReadAllText(completeFileName));
					errorString = null;
					return true;
				}
				catch (Exception ex)
				{
					errorString = $"{ex}";
					return false;
				}
			}
			else
			{
				errorString = $"{nameof(fileName)} cannot be null";
				return false;
			}
		}

		/// <summary>
		/// Converts <see cref="SongKey"/> to a list of <see cref="NoteGroup"/>
		/// </summary>
		/// <param name="song"></param>
		public void ConvertSongNotesToNoteSequence(SongKey song)
		{
			Song = song;
			NoteSequence = CreatePlayableNotesFromSongNotes(Song.SongNotes);
		}

		/// <summary>
		/// Converts <see cref="Tablature"/> to a list of <see cref="NoteGroup"/> using <see cref="IStringInstrument"/> to <see cref="SongKey"/>
		/// </summary>
		/// <param name="guitar"></param>
		/// <param name="tabDefinition"></param>
		public void ConvertTabsToNoteSequence(IStringInstrument guitar, Tablature tabDefinition)
		{
			var frettedNotes = tabDefinition.TabString.Split(',');
			Song = new SongKey(tabDefinition.Name, null);
			NoteSequence = CreatePlayableNotesFromTabs(guitar, frettedNotes);
		}

		#endregion Public methods

		#region Event Handlers

		private void SongNoteDuration_Error(object sender, Exception e)
		{
			PlayableSequenceEvent?.Invoke(this, new PlayableSequenceEvent() { EventType = PlayableEventType.Error, EventDetails = $"{e}" });
		}

		private void SongNoteDuration_PlayingNote(object sender, EventArgs e)
		{
			if (sender is MusicNote musicNote)
			{
				var message = $"Playing note {musicNote.Key}{musicNote.DesiredOctave} at {musicNote.Frequencies[musicNote.DesiredOctave]} Hz for {musicNote.Duration}ms as {musicNote.Instrument}.";
				PlayableSequenceEvent?.Invoke(this, new PlayableSequenceEvent() { EventType = PlayableEventType.PlayingNote, EventDetails = message });
			}
		}

		#endregion Event Handlers

		#region Private helpers

		#region Tabs processing

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabbedNotes"></param>
		/// <returns></returns>
		private IEnumerable<NoteGroup> CreatePlayableNotesFromSongNotes(IEnumerable<string> songNotes)
		{
			foreach (var currentNote in songNotes)
			{
				var multipleNotes = currentNote.Split('|');
				var timedSongNotes = GetTimedSongNotes(multipleNotes);
				yield return new NoteGroup() { Notes = timedSongNotes.ToArray(), PrimaryNote = timedSongNotes.First() };
			}
		}

		private IEnumerable<string> GetTimedSongNotes(string[] multipleNotes)
		{
			foreach (var singleNote in multipleNotes)
			{
				var noteParts = singleNote.Split('-');
				var songNote = GetSongNote(noteParts);
				yield return songNote;
			}
		}

		/// <summary>
		/// Converts a string array of fretted notes to  using <see cref="Guitar"/> to 
		/// </summary>
		/// <param name="guitar"></param>
		/// <param name="tabbedNotes"></param>
		/// <returns></returns>
		private IEnumerable<NoteGroup> CreatePlayableNotesFromTabs(IStringInstrument guitar, IEnumerable<string> tabbedNotes)
		{
			foreach (var currentNote in tabbedNotes)
			{
				var multipleNotes = currentNote.Split('|');
				var noteList = GetNoteList(guitar, multipleNotes);
				yield return new NoteGroup() { Notes = noteList.ToArray(), PrimaryNote = noteList.First() };
			}
		}

		/// <summary>
		/// Frets the notes on the <see cref="IStringInstrument"/> and gets back the note values 
		/// </summary>
		/// <param name="guitar"></param>
		/// <param name="multipleNotes"></param>
		/// <returns></returns>
		private IEnumerable<string> GetNoteList(IStringInstrument guitar, string[] multipleNotes)
		{
			foreach (var singleNote in multipleNotes)
			{
				yield return GetNoteFromGuitar(guitar, singleNote);
			}
		}

		/// <summary>
		/// Frets a single noite on the <see cref="IStringInstrument"/>
		/// </summary>
		/// <param name="guitar"></param>
		/// <param name="singleNote"></param>
		/// <returns></returns>
		private string GetNoteFromGuitar(IStringInstrument guitar, string singleNote)
		{
			var noteParts = singleNote.Split('-');
			var stringNum = GetString(noteParts);
			var fret = GetFret(noteParts);
			var timing = GetNoteDuration(noteParts);
			var note = guitar.GetNote(stringNum, fret);
			return $"{note}-{timing}";
		}

		private IEnumerable<CompositeNote> GetCompositeNotesFromSequence()
		{
			foreach (var noteSequence in NoteSequence)
			{
				yield return new CompositeNote(InstrumentType, NotePlayer, noteSequence.Notes, A4Reference);
			}
		}


		/// <summary>
		/// Get the String number that is being fretted
		/// </summary>
		/// <param name="arry"></param>
		/// <returns></returns>
		private int GetString(string[] arry)
		{
			var stringNum = 0;
			if (arry.Length > 0)
			{
				stringNum = int.Parse(arry[0]);
			}
			return stringNum;
		}

		/// <summary>
		/// Gets the Fret number
		/// </summary>
		/// <param name="arry"></param>
		/// <returns></returns>
		private int GetFret(string[] arry)
		{
			var fret = 0;
			if (arry.Length > 1)
			{
				fret = int.Parse(arry[1]);
			}

			return fret;
		}

		private string GetSongNote(string[] arry)
		{
			var note = string.Empty;
			var timing = MeasureTime;
			if (arry.Length > 0)
			{
				note = arry[0];

				// check for Octave
				if(!note.Any(ch => char.IsDigit(ch)))
				{
					note = $"{note}{Octave}";
				}
				else
				{
					note = arry[0];
				}
			}
			if (arry.Length > 1)
			{
				var noteTime = float.Parse(arry[1]);
				timing = (int)(MeasureTime * noteTime);
			}
			return $"{note}-{timing}";
		}

		/// <summary>
		/// Gets the note duration as ms based on the measure time percentage the note plays for
		/// </summary>
		/// <param name="arry"></param>
		/// <returns></returns>
		private int GetNoteDuration(string[] arry)
		{
			if (arry.Length > 2)
			{
				return (int)(MeasureTime * (float.Parse(arry[2])));
			}
			else
			{
				return MeasureTime;
			}
		}

		/// <summary>
		/// Gets the Instrument 
		/// </summary>
		/// <param name="arry"></param>
		/// <returns></returns>
		private InstrumentType GetInstrument(string[] arry)
		{
			if (arry.Length == 3)
			{
				return (InstrumentType)int.Parse(arry[2]);
			}
			return InstrumentType;
		}

		#endregion Tabs processing

		#endregion Private helpers
	}
}
