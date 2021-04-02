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

		public string[] Song { get; set; }
		public IEnumerable<MusicNote> NoteSequence { get; private set; }
		public int MeasureTime { get; set; } = 1000;
		public int Octave { get; set; } = 2;
		public InstrumentType InstrumentType { get; set; } = InstrumentType.Horn;
		public int A4Reference { get; set; } = 440;
		public IPlayer NotePlayer { get; private set; }
		public IStringInstrument Guitar { get; private set; }
		public int? Repeat { get; set; }

		#endregion Public properties

		#region Events

		public delegate void PlayableSequenceEventHandler(object sender, PlayableSequenceEvent e);
		public event PlayableSequenceEventHandler PlayableSequenceEvent;

		#endregion Events

		#region Public methods

		public void Prepare()
		{
			NotePlayer = new SignalNotePlayer();
			Guitar = new Guitar();
			PrepareSequence();
		}

		/// <summary>
		/// Turns the string notes in <see cref="Song"/> into <see cref="MusicNote"/>s
		/// </summary>
		public void PrepareSequence()
		{
			var currentInstrument = InstrumentType;
			var sequence = Song.Select(s => s.Split(',')[0]).Select(songDef =>
			{
				var arry = songDef.Split('-');
				var currentNote = arry[0];
				var rawNote = arry[0];
				currentInstrument = GetInstrument(arry);
				var currentOctave = Octave;
				if (currentNote.Any(ch => char.IsDigit(ch)))
				{
					var octaveString = new String(currentNote.Where(ch => char.IsDigit(ch)).ToArray());
					var noteOctave = int.Parse(octaveString);
					currentOctave += noteOctave;
					rawNote = currentNote.Substring(0, currentNote.IndexOf(octaveString));
				}
				currentNote = $"{rawNote}{currentOctave}";
				return MusicNote.Create(currentNote, A4Reference, NotePlayer, GetNoteDuration(arry), currentInstrument);
			});
			NoteSequence = sequence;
		}

		/// <summary>
		/// Plays the sequence of notes in <see cref="NoteSequence"/>
		/// </summary>
		public void Play()
		{
			var repeat = Repeat ?? 1;
			var message = $"Playing sequence of {NoteSequence.Count()} notes {repeat} time(s).";
			PlayableSequenceEvent?.Invoke(this, new PlayableSequenceEvent() { EventType = PlayableEventType.StartSequence, EventDetails = message });
			for (var counter = repeat; counter > 0; counter--)
			{
				foreach (var songNoteDuration in NoteSequence)
				{
					try
					{
						songNoteDuration.PlayingNote += SongNoteDuration_PlayingNote;
						songNoteDuration.Error += SongNoteDuration_Error;
						songNoteDuration.PlayNote();
					}
					catch (Exception ex)
					{
						PlayableSequenceEvent?.Invoke(this, new PlayableSequenceEvent() { EventType = PlayableEventType.Error, EventDetails = $"{ex}" });
					}
				}
			}
			PlayableSequenceEvent?.Invoke(this, new PlayableSequenceEvent() { EventType = PlayableEventType.StopSequence, EventDetails = $"Finished playing {NoteSequence.Count()} notes." });
		}

		/// <summary>
		/// Loads a sequence of octave weighted, timed and instrument specific notes from a string into <see cref="NoteSequence"/> 
		/// </summary>
		/// <param name="sequence"></param>
		public void LoadSequenceFromString(IEnumerable<string> sequence)
		{
			Song = sequence.ToArray();
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
		/// Converts <see cref="Tablature"/> using <see cref="Guitar"/> to <see cref="Song"/>
		/// </summary>
		/// <param name="guitar"></param>
		/// <param name="tabDefinition"></param>
		public void ConvertTabsToSongNotes(Guitar guitar, Tablature tabDefinition)
		{
			var frettedNotes = tabDefinition.TabString.Split(',');
			Song = CreatePlayableNotesFromTabs(guitar, frettedNotes).ToArray();
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

		/// <summary>
		/// Converts a string array of fretted notes to  using <see cref="Guitar"/> to 
		/// </summary>
		/// <param name="guitar"></param>
		/// <param name="tabbedNotes"></param>
		/// <returns></returns>
		private IEnumerable<string> CreatePlayableNotesFromTabs(Guitar guitar, IEnumerable<string> tabbedNotes)
		{
			foreach (var currentNote in tabbedNotes)
			{
				var fret = 0;
				var timing = 1.0;
				var noteParts = currentNote.Split('-');
				var stringName = noteParts[0];
				if (noteParts.Length > 1)
				{
					fret = int.Parse(noteParts[1]);
				}
				if (noteParts.Length > 2)
				{
					timing = float.Parse(noteParts[2]);
				}
				var note = guitar.GetNote(int.Parse(stringName), fret);
				yield return $"{note}-{timing}";
			}
		}

		private int GetNoteDuration(string[] arry)
		{
			if (arry.Length > 1)
			{
				return (int)(MeasureTime * (float.Parse(arry[1])));
			}
			else
			{
				return MeasureTime;
			}
		}

		private InstrumentType GetInstrument(string[] arry)
		{
			if (arry.Length == 3)
			{
				return (InstrumentType)int.Parse(arry[2]);
			}
			return InstrumentType;
		}

		#endregion Private helpers
	}
}
