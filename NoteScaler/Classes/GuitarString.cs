namespace NoteScaler.Classes
{
	using BidirectionalMap;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class GuitarString : IInstrumentString
	{
		public const int NOTE_NOT_FOUND = -1;

		/// <summary>
		/// One guitar string on a guitar
		/// </summary>
		/// <param name="number"></param>
		/// <param name="tuning"></param>
		/// <param name="frets"></param>
		public GuitarString(int number, string tuning, int frets)
		{
			Number = number;
			Tuning = tuning;
			MaxFret = frets;
			NoteMap = new BiMap<string, int>();
			InitializeFretBoard();
		}

		/// <summary>
		/// String number 
		/// </summary>
		public int Number { get; }

		/// <summary>
		/// Note that the string is tuned for Fret 0 (open)
		/// </summary>
		public string Tuning { get; protected set; }

		/// <summary>
		/// Max number of frets on the string
		/// </summary>
		public int MaxFret { get; }

		/// <summary>
		/// Bi-directional mapping of Note <-> Fret Number for this <see cref="GuitarString"/>
		/// </summary>
		protected BiMap<string, int> NoteMap { get; }

		/// <summary>
		/// When the tuning changes the Fretboard for that string changes
		/// </summary>
		/// <param name="noteToTune"></param>
		public void SetTuning(string noteToTune)
		{
			Tuning = noteToTune;
			InitializeFretBoard();
		}

		/// <summary>
		/// Gets the Fret number of the specified note
		/// </summary>
		/// <param name="note"></param>
		/// <returns></returns>
		public int GetNote(string note)
		{
			if (NoteMap.Forward.ContainsKey(note))
			{
				return NoteMap.Forward[note];
			}
			return NOTE_NOT_FOUND;
		}

		/// <summary>
		/// Get the note that is played on the Fret number
		/// </summary>
		/// <param name="fret"></param>
		/// <returns></returns>
		public string GetNote(int fret)
		{
			if (NoteMap.Reverse.ContainsKey(fret))
			{
				return NoteMap.Reverse[fret];
			}
			return null;
		}

		/// <summary>
		/// Gets a list of all the notes on the String
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetNotesInString()
		{
			return NoteMap.Forward.Select(ntf => $"{ntf.Value}:{ntf.Key}");
		}

		/// <summary>
		/// Human readable guitar string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Join(',', GetNotesInString());
		}

		private void InitializeFretBoard()
		{
			var startingNote = Array.FindIndex(NoteIndex.Notes, note => note.Equals(Tuning));
			var notes = NoteIndex.Notes.Skip(startingNote).Take(MaxFret + 1);
			var startingIndex = 0;
			foreach (var note in notes)
			{
				NoteMap.Add(note, startingIndex);
				startingIndex++;
			}
		}
	}
}