
namespace NoteScaler
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MusicNote
	{
		#region private static data
		/// <summary>
		/// Base scale using flats
		/// </summary>
		private static readonly string[] NotesFlat = new string[] { "A", "Bb", "B", "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A" };

		/// <summary>
		/// Base scale using sharps
		/// </summary>
		private static readonly string[] NotesSharp = new string[] { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A" };

		/// <summary>
		/// Pattern for Major scale
		/// </summary>
		private static readonly string MAJOR_SCALE = "W,W,H,W,W,W,H";

		/// <summary>
		/// Pattern for minor scale
		/// </summary>
		private static readonly string MINOR_SCALE = "W,H,W,W,H,W,W";

		#endregion private static data

		#region public properties

		/// <summary>
		/// Name of the note
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Tone type derived from Note
		/// </summary>
		public ToneTypes ToneType { get; }
		
		/// <summary>
		/// Set of flat notes and natural notes in this key
		/// </summary>
		public IEnumerable<string> FlatNotes { get; private set; }

		/// <summary>
		/// Set of sharp notes and natural notes in this key
		/// </summary>
		public IEnumerable<string> SharpNotes { get; private set; }

		/// <summary>
		/// Relative minor note to this key
		/// </summary>
		public MusicNote RelativeMinor => GetRelativeKey(false);

		/// <summary>
		/// Get the relative major to the minor key, should be this key!
		/// </summary>
		public MusicNote RelativeMajor => GetRelativeKey(true);

		/// <summary>
		/// Major scale for the current Key
		/// </summary>
		public IEnumerable<MusicNote> MajorScale => GetMajorScale().Select(note => new MusicNote(note));

		/// <summary>
		/// minor scale for the current Key
		/// </summary>
		public IEnumerable<MusicNote> MinorScale => GetMinorScale().Select(note => new MusicNote(note));

		#region Key melodic properties
		/// <summary>
		/// Is this a natural key/note?
		/// </summary>
		public bool IsNatural => !Key.EndsWith("#") && !Key.EndsWith("b");
		
		/// <summary>
		/// Is this a flat key/note?
		/// </summary>
		public bool IsFlat => Key.EndsWith("b");

		/// <summary>
		/// Is this a sharp key/note?
		/// </summary>
		public bool IsSharp => Key.EndsWith("#");
		#endregion Key melodic properties

		#region Note positions in note scale
		
		/// <summary>
		/// Key before this one
		/// </summary>
		public MusicNote NoteBefore => GetNoteBefore(false);

		/// <summary>
		/// Key after this one
		/// </summary>
		public MusicNote NoteAfter => GetNoteAfter(false);

		#endregion Note positions in note scale

		#region Note positions inside key 
		
		/// <summary>
		/// Major scale note before this one in key
		/// </summary>
		public MusicNote MajorNoteBefore => GetNoteBefore(true);

		/// <summary>
		/// Major scale note after this one in key
		/// </summary>
		public MusicNote MajorNoteAfter => GetNoteAfter(true);

		/// <summary>
		/// minor scale note before this one in key
		/// </summary>
		public MusicNote MinorNoteBefore => GetNoteBefore(true, true);

		/// <summary>
		/// minor scale note after this one in key
		/// </summary>
		public MusicNote MinorNoteAfter => GetNoteAfter(true, true);
		#endregion Note positions inside key 

		#region Chords for Key

		/// <summary>
		/// Major Chrod out to 15th degree
		/// </summary>
		public string[] MajorChord => GetMajorChord();

		/// <summary>
		/// Minor Chord out to 15th degree
		/// </summary>
		public string[] MinorChord => GetMinorChord();

		#endregion Chords for Key

		/// <summary>
		/// Human readable form
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Key}";
		}

		#endregion public properties

		#region Ctor

		/// <summary>
		/// Onlt way to construct a Key for a note
		/// </summary>
		/// <param name="note"></param>
		public MusicNote(string note)
		{
			if(note.EndsWith("#"))
			{
				ToneType = ToneTypes.Sharp;
			}
			else if(note.EndsWith("b"))
			{
				ToneType = ToneTypes.Flat;
			}
			else
			{
				ToneType = ToneTypes.Natural;
			}
			Key = note;

			InitializeNote();
		}

		#endregion Ctor

		#region Public methods
		public void MakeSharper()
		{
			this.Key = NoteAfter.Key;
			InitializeNote();
		}
		public void MakeFlatter()
		{
			this.Key = NoteBefore.Key;
			InitializeNote();
		}

		#endregion Public methods

		#region private helpers 

		private void InitializeNote()
		{
			var sharpNotes = NotesSharp.ToList();
			var flatNotes = NotesFlat.ToList();
			var currentSharpNoteIndex = sharpNotes.IndexOf(Key);
			var currentFlatNoteIndex = flatNotes.IndexOf(Key);
			int currentNoteIndex = currentSharpNoteIndex == -1 ? currentFlatNoteIndex : currentSharpNoteIndex;

			if (currentNoteIndex > 0)
			{
				var takeFlat = flatNotes.Count - currentNoteIndex - 1;
				var notes = flatNotes.Skip(currentNoteIndex).Take(takeFlat).ToList();
				notes.AddRange(flatNotes.Take(currentNoteIndex + 1));
				FlatNotes = notes;
	
				var takeSharp = sharpNotes.Count - currentNoteIndex - 1;
				notes = sharpNotes.Skip(currentNoteIndex).Take(takeSharp).ToList();
				notes.AddRange(sharpNotes.Take(currentNoteIndex + 1));
				SharpNotes = notes;
			}
			else
			{
				FlatNotes = flatNotes;
				SharpNotes = sharpNotes;
			}
		}
		private MusicNote GetRelativeKey(bool forMajor = false)
		{
			string targetKey;

			if (!forMajor)
			{
				targetKey = MajorScale.ElementAt(5).Key;
			}
			else
			{
				targetKey = RelativeMinor.MinorScale.ElementAt(2).Key;
			}
			var newMusicNote = new MusicNote(targetKey);
			return newMusicNote;
		}
		private IEnumerable<string> GetMajorScale()
		{
			// Formula WWHWWWH
			var scale =  GetScale(true, ToneType == ToneTypes.Flat);
			return CorrectScales(scale.Select(s => new MusicNote(s))).Select(scale => scale.Key);
		}

		private IEnumerable<string> GetMinorScale()
		{
			// Formula WHWWHWW
			var scale = GetScale(false, ToneType == ToneTypes.Flat);
			return CorrectScales(scale.Select(s => new MusicNote(s))).Select(scale => scale.Key);
		}

		private IEnumerable<MusicNote> CorrectScales(IEnumerable<MusicNote> scale)
		{
			var noteList = new List<MusicNote>();
			noteList.AddRange(scale);

			noteList.GroupBy(scale => scale.Key.First().ToString()).Where(g => g.Count() > 1).Select(g => g.Key).ToList().ForEach(key =>
			{
				noteList.Where(s => s.Key.StartsWith(key) && s.Key != Key).ToList().ForEach(note =>
				{
					if (IsSharp)
					{
						note.MakeFlatter();
					}
					else if(IsFlat)
					{
						note.MakeSharper();
					}

				});
			});

			return noteList;
		}
		private MusicNote GetNoteBefore(bool useScale = true, bool useMinorScale = false)
		{
			var noteList = GetNoteList(useScale, useMinorScale);
			var currentPos = Array.FindIndex(noteList.ToArray(), nl => nl.Equals(Key));
			var targetPos = currentPos == 0 ? noteList.Count() - 2 : currentPos - 1;
			var targetNote = noteList.ElementAt(targetPos);
			return new MusicNote(targetNote);
		}

		private MusicNote GetNoteAfter(bool useScale = true, bool useMinorScale = false)
		{
			var noteList = GetNoteList(useScale, useMinorScale);
			var currentPos = Array.FindIndex(noteList.ToArray(), nl => nl.Equals(Key));
			var targetPos = currentPos == 12 ? 0 : currentPos + 1;
			var targetNote = noteList.ElementAt(targetPos);
			return new MusicNote(targetNote);
		}

		private string[] GetMajorChord()
		{
			List<string> chordsForNote = new List<string>();
			var noteList = GetNoteList(true).ToList();
			chordsForNote.Add(noteList[0]);
			chordsForNote.Add(noteList[2]);
			chordsForNote.Add(noteList[4]);
			chordsForNote.Add(noteList[6]);
			chordsForNote.Add(noteList[1]);
			chordsForNote.Add(noteList[3]);
			chordsForNote.Add(noteList[5]);
			chordsForNote.Add(noteList[7]);
			return chordsForNote.ToArray();
		}

		private string[] GetMinorChord()
		{
			List<string> chordsForNote = new List<string>();
			var noteList = GetNoteList(true, true).ToList();
			chordsForNote.Add(noteList[0]);
			chordsForNote.Add(noteList[2]);
			chordsForNote.Add(noteList[4]);
			chordsForNote.Add(noteList[6]);
			chordsForNote.Add(noteList[1]);
			chordsForNote.Add(noteList[3]);
			chordsForNote.Add(noteList[5]);
			chordsForNote.Add(noteList[7]);
			return chordsForNote.ToArray();
		}

		private IEnumerable<string> GetNoteList(bool useScale = false, bool useMinorScale = false)
		{
			IEnumerable<string> noteList;
			if (useScale)
			{
				if (useMinorScale)
				{
					noteList = GetMinorScale();
				}
				else
				{
					noteList = GetMajorScale();
				}
			}
			else
			{
				noteList = IsNatural ? NotesFlat : IsSharp ? NotesSharp : NotesFlat;
			}

			return noteList;
		}

		private IEnumerable<string> GetScale(bool isMajor = true, bool useFlats = false)
		{
			List<string> scaleNotes;
			var noteList = new List<string>();
			if (useFlats)
			{
				scaleNotes = FlatNotes.ToList();
			}
			else
			{
				 scaleNotes = SharpNotes.ToList();
			}

			int currentNotesInScale = 0;
			int notesCounted = 0;
			var currentScale = MAJOR_SCALE.Split(',');
			if (!isMajor)
			{
				currentScale = MINOR_SCALE.Split(',');
			}
			noteList.Add(Key);
			string lastNote = null;
			while (noteList.Count() < 8)
			{
				int index = 1;
				if (currentScale[currentNotesInScale] == "W" && (scaleNotes[notesCounted] != "B" || scaleNotes[notesCounted] != "E"))
				{
					index = 2;
				}
				notesCounted += index;
				var newNote = scaleNotes.ElementAt(notesCounted);
				if((lastNote?.Length ?? 0) > 1 && (lastNote?.StartsWith(newNote[0]) ?? false))
				{
					if(useFlats)
					{
						newNote = SharpNotes.ElementAt(notesCounted);
					}
					else
					{
						newNote = FlatNotes.ElementAt(notesCounted);
					}
				}
				noteList.Add(newNote);
				lastNote = newNote;
				currentNotesInScale++;
			}
			return noteList;
		}

		#endregion private helpers 
	}
}
