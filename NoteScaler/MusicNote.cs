namespace NoteScaler
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MusicNote
	{
		#region Constants

		private const int RELATIVE_MINOR_POSITION = 5;
		private const int RELATIVE_MAJOR_POSITION = 2;
		private const int FIRST = 0;
		private const int THIRD = 2;
		private const int FIFTH = 4;
		private const int SEVENTH = 6;
		private const int NINTH = 1;
		private const int ELEVENTH = 3;
		private const int THIRTEENTH = 5;
		private const int FIFTEENTH = 7;
		private const string B_NOTE = "B";
		private const string C_NOTE = "C";
		private const string E_NOTE = "E";
		private const string WHOLE_STEP = "W";
		private const string HALF_STEP = "H";
		private const string SHARP_NOTE = "#";
		private const string FLAT_NOTE = "b";
		private const char Separator = ',';

		#endregion Constants

		#region private static data

		/// <summary>
		/// Multiplier for notes away from A4
		/// </summary>
		private static readonly double NOTE_REF = 1.059463;

		/// <summary>
		/// Base scale using flats
		/// </summary>
		private static readonly string[] NotesFlat = new string[] { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B", "C" };

		/// <summary>
		/// Base scale using sharps
		/// </summary>
		private static readonly string[] NotesSharp = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "C" };

		/// <summary>
		/// Pattern for Major scale
		/// </summary>
		private static readonly string MAJOR_SCALE = $"{WHOLE_STEP},{WHOLE_STEP},{HALF_STEP},{WHOLE_STEP},{WHOLE_STEP},{WHOLE_STEP},{HALF_STEP}";

		/// <summary>
		/// Pattern for minor scale
		/// </summary>
		private static readonly string MINOR_SCALE = $"{WHOLE_STEP},{HALF_STEP},{WHOLE_STEP},{WHOLE_STEP},{HALF_STEP},{WHOLE_STEP},{WHOLE_STEP}";

		#endregion private static data

		#region private data
		private int desiredOctave = 0;
		private IEnumerable<string> minorScale;
		private IEnumerable<string> relativeMinorScale;
		private IEnumerable<string> majorScale;
		private string relativeMinor;
		private string relativeMajor;
		private string noteBefore;
		private string noteAfter;
		private string minorNoteBefore;
		private string minorNoteAfter;
		private string majorNoteBefore;
		private string majorNoteAfter;
		private string[] majorChord;
		private string[] minorChord;
		#endregion private data

		#region public properties

		/// <summary>
		/// Name of the note
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// The frequency of the A4 note 
		/// </summary>
		public int Reference { get; set; }

		/// <summary>
		/// Tone type derived from Note
		/// </summary>
		public ToneTypes ToneType { get; private set; }
		
		/// <summary>
		/// Set of flat notes and natural notes in this key
		/// </summary>
		public IEnumerable<string> FlatNotes { get; private set; }

		/// <summary>
		/// Frequencies for this note in each Octave
		/// </summary>
		public float[] Frequencies { get; set; }

		/// <summary>
		/// Set of sharp notes and natural notes in this key
		/// </summary>
		public IEnumerable<string> SharpNotes { get; private set; }

		/// <summary>
		/// How long to play the note for
		/// </summary>
		public int Duration { get; set; }

		/// <summary>
		/// Instrument used to play the note
		/// </summary>
		public InstrumentType Instrument { get; set; }

		#region Key melodic properties
		/// <summary>
		/// Is this a natural key/note?
		/// </summary>
		public bool IsNatural => !Key.Contains(SHARP_NOTE) && !Key.Contains(FLAT_NOTE);
		
		/// <summary>
		/// Is this a flat key/note?
		/// </summary>
		public bool IsFlat => Key.Contains(FLAT_NOTE);

		/// <summary>
		/// Is this a sharp key/note?
		/// </summary>
		public bool IsSharp => Key.Contains(SHARP_NOTE);

		/// <summary>
		/// The octave desired when this note is played
		/// </summary>
		public int DesiredOctave => desiredOctave;

		/// <summary>
		/// Interface for playing notes
		/// </summary>
		public INotePlayer NotePlayer { get; set; }

		#endregion Key melodic properties

		#region Scale data

		/// <summary>
		/// Relative minor note to this key
		/// </summary>
		public string RelativeMinor => relativeMinor;

		/// <summary>
		/// Get the relative major to the minor key, should be this key!
		/// </summary>
		public string RelativeMajor => relativeMajor;

		/// <summary>
		/// Major scale for the current Key
		/// </summary>
		public IEnumerable<string> MajorScale => majorScale;

		/// <summary>
		/// minor scale for the current Key
		/// </summary>
		public IEnumerable<string> MinorScale => minorScale;

		#endregion Scale data

		#region Note positions in note scale

		/// <summary>
		/// Key before this one
		/// </summary>
		public string NoteBefore => noteBefore;

		/// <summary>
		/// Key after this one
		/// </summary>
		public string NoteAfter => noteAfter;

		#endregion Note positions in note scale

		#region Note positions inside key 
		
		/// <summary>
		/// Major scale note before this one in key
		/// </summary>
		public string MajorNoteBefore => majorNoteBefore;

		/// <summary>
		/// Major scale note after this one in key
		/// </summary>
		public string MajorNoteAfter => majorNoteAfter;

		/// <summary>
		/// minor scale note before this one in key
		/// </summary>
		public string MinorNoteBefore => minorNoteBefore;

		/// <summary>
		/// minor scale note after this one in key
		/// </summary>
		public string MinorNoteAfter => minorNoteAfter;
		#endregion Note positions inside key 

		#region Chords for Key

		/// <summary>
		/// Major Chord out to 15th degree
		/// </summary>
		public string[] MajorChord => majorChord;

		/// <summary>
		/// Minor Chord out to 15th degree
		/// </summary>
		public string[] MinorChord => minorChord;

		#endregion Chords for Key

		/// <summary>
		/// Human readable form
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Key}{DesiredOctave}";
		}

		#endregion public properties

		#region Ctor

		/// <summary>
		/// Onlt way to construct a Key for a note
		/// </summary>
		/// <param name="note"></param>
		/// <param name="a4Reference"></param>
		/// <param name="player"></param>
		/// <param name="duration"></param>
		/// <param name="currentInstrument"></param>
		public MusicNote(string note, int a4Reference = 440, INotePlayer player = null, int duration = 500, InstrumentType currentInstrument = default)
		{
			Key = note;
			Reference = a4Reference;
			this.NotePlayer = player;
			this.Duration = duration;
			this.Instrument = currentInstrument;
			InitializeNote();
		}

		#endregion Ctor

		#region Public methods

		public void PlayNote(int duration)
		{
			if(DesiredOctave > Frequencies.Count() -1)
			{
				throw new ArgumentException($"{DesiredOctave} is too high of an Octave to play {Key}", nameof(DesiredOctave));
			}
			var frequency = Frequencies[DesiredOctave];
			Console.WriteLine($"Playing note {this} at {frequency} Hz for {duration}ms as {Instrument}.");
			NotePlayer?.PlayNote(Frequencies[DesiredOctave], Instrument, duration);
		}

		public void MakeSharper()
		{
			this.Key = NoteAfter;
			InitializeNote();
		}
		public void MakeFlatter()
		{
			this.Key = NoteBefore;
			InitializeNote();
		}

		#endregion Public methods

		#region private helpers 

		private void InitializeNote()
		{
			SetToneOfNote();
			AdjustNoteOctave();
			GetNoteIndex(out List<string> sharpNotes, out List<string> flatNotes, out int currentNoteIndex);

			if (currentNoteIndex > 0)
			{
				var takeFlat = flatNotes.Count - currentNoteIndex - 1;
				var notes = flatNotes.Skip(currentNoteIndex).Take(takeFlat).ToList();
				notes.AddRange(flatNotes.Take(currentNoteIndex + 1).Select(note => $"{note}"));
				FlatNotes = notes;

				var takeSharp = sharpNotes.Count - currentNoteIndex - 1;
				notes = sharpNotes.Skip(currentNoteIndex).Take(takeSharp).ToList();
				notes.AddRange(sharpNotes.Take(currentNoteIndex + 1).Select(note => $"{note}"));
				SharpNotes = notes;
			}
			else
			{
				FlatNotes = flatNotes;
				SharpNotes = sharpNotes;
			}

			var frequencyList = GetFrequenciesFromReference().ToArray();
			Frequencies = new float[]
			{
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,-4),
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,-3),
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,-2),
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,-1),
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,0),
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,1),
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,2),
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,3),
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,4),
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,5),
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,6),
				frequencyList[currentNoteIndex]*(float) Math.Pow(2,7)
			};

			minorScale = GetMinorScale();
			majorScale = GetMajorScale();
			relativeMinor = GetRelativeKey(false);
			relativeMajor = GetRelativeKey(true);
			noteBefore = GetNoteBefore(false);
			noteAfter = GetNoteAfter(false);
			minorNoteBefore = GetNoteBefore(true, true);
			minorNoteAfter = GetNoteAfter(true, true);
			majorNoteBefore = GetNoteBefore(true);
			majorNoteAfter = GetNoteAfter(true);
			minorChord = GetMinorChord();
			majorChord = GetMajorChord();
		}

		private float[] GetFrequenciesFromReference()
		{
			var frequencyList = new float[NotesSharp.Length];
			var a4Pos = Array.IndexOf(NotesSharp, "A");
			NotesSharp.ToList().ForEach(note =>
			{
				var currentIndex = Array.IndexOf(NotesSharp, note);
				double index = currentIndex - a4Pos;
				if (index == 0)
				{
					frequencyList[currentIndex] = Reference;
				}
				else
				{
					frequencyList[currentIndex] = (float)(Reference * Math.Pow(NOTE_REF, index));
				}
			});
			frequencyList[^1] = (float) (Reference * Math.Pow(NOTE_REF, 3));
			return frequencyList;
		}

		private void GetNoteIndex(out List<string> sharpNotes, out List<string> flatNotes, out int currentNoteIndex)
		{
			sharpNotes = NotesSharp.ToList();
			flatNotes = NotesFlat.ToList();
			var currentSharpNoteIndex = sharpNotes.IndexOf(Key);
			var currentFlatNoteIndex = flatNotes.IndexOf(Key);
			currentNoteIndex = currentSharpNoteIndex == -1 ? currentFlatNoteIndex : currentSharpNoteIndex;
			if (currentNoteIndex == -1)
			{
				throw new ArgumentException("Unable to find referenced note!", Key);
			}
		}

		private void AdjustNoteOctave()
		{
			var testKey = Key;
			if (testKey.Any(ch => char.IsDigit(ch)))
			{
				var rawNote = new String(testKey.Where(ch => !char.IsDigit(ch)).ToArray());
				var octaveString = new String(testKey.Where(ch => char.IsDigit(ch)).ToArray());
				desiredOctave = int.Parse(octaveString);
				Key = rawNote;
			}
		}

		private void SetToneOfNote()
		{
			if (Key.Contains(SHARP_NOTE))
			{
				ToneType = ToneTypes.Sharp;
			}
			else if (Key.Contains(FLAT_NOTE))
			{
				ToneType = ToneTypes.Flat;
			}
			else
			{
				ToneType = ToneTypes.Natural;
			}
		}

		private string GetRelativeKey(bool forMajor = false)
		{
			string targetKey;

			if (!forMajor)
			{
				targetKey = MajorScale.ElementAt(RELATIVE_MINOR_POSITION);
			}
			else
			{
				targetKey = MajorScale.ElementAt(RELATIVE_MAJOR_POSITION);
			}
			return targetKey;
		}
		private IEnumerable<string> GetMajorScale()
		{
			// Formula WWHWWWH
			return GetScale(true, ToneType == ToneTypes.Flat);
		}

		private IEnumerable<string> GetMinorScale()
		{
			// Formula WHWWHWW
			return GetScale(false, ToneType == ToneTypes.Flat);
		}

		private string GetNoteBefore(bool useScale = true, bool useMinorScale = false)
		{
			var noteList = GetNoteList(useScale, useMinorScale);
			var currentPos = Array.FindIndex(noteList.ToArray(), nl => nl.Contains(Key));
			var targetPos = currentPos == 0 ? noteList.Count() - 2 : currentPos - 1;
			var targetNote = noteList.ElementAt(targetPos);
			return targetNote;
		}

		private string GetNoteAfter(bool useScale = true, bool useMinorScale = false)
		{
			var noteList = GetNoteList(useScale, useMinorScale);
			var currentPos = Array.FindIndex(noteList.ToArray(), nl => nl.Contains(Key));
			var targetPos = currentPos == 12 ? 0 : currentPos + 1;
			var targetNote = noteList.ElementAt(targetPos);
			return targetNote;
		}

		private string[] GetMajorChord()
		{
			List<string> chordsForNote = new List<string>();
			var noteList = GetNoteList(true).ToList();
			return FillNotes(chordsForNote, noteList);
		}

		private string[] GetMinorChord()
		{
			List<string> chordsForNote = new List<string>();
			var noteList = GetNoteList(true, true).ToList();
			return FillNotes(chordsForNote, noteList);
		}

		private static string[] FillNotes(List<string> chordsForNote, List<string> noteList)
		{
			chordsForNote.Add(noteList[FIRST]);
			chordsForNote.Add(noteList[THIRD]);
			chordsForNote.Add(noteList[FIFTH]);
			chordsForNote.Add(noteList[SEVENTH]);
			chordsForNote.Add(noteList[NINTH]);
			chordsForNote.Add(noteList[ELEVENTH]);
			chordsForNote.Add(noteList[THIRTEENTH]);
			chordsForNote.Add(noteList[FIFTEENTH]);
			return chordsForNote.ToArray();
		}

		private IEnumerable<string> GetNoteList(bool useScale = false, bool useMinorScale = false)
		{
			IEnumerable<string> noteList;
			if (useScale)
			{
				if (useMinorScale)
				{
					noteList = minorScale;
				}
				else
				{
					noteList = majorScale;
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
			int currentOctave = 0;
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
			var currentScale = MAJOR_SCALE.Split(Separator);
			if (!isMajor)
			{
				currentScale = MINOR_SCALE.Split(Separator);
			}
			noteList.Add($"{Key}{currentOctave}");
			string lastNote = null;
			while (noteList.Count() < 8)
			{
				int index = 1;
				if (currentScale[currentNotesInScale] == WHOLE_STEP && (scaleNotes[notesCounted] != B_NOTE || scaleNotes[notesCounted] != E_NOTE))
				{
					index = 2;
				}
				notesCounted += index;
				var newNote = scaleNotes.ElementAt(notesCounted);
				/*
				if((lastNote?.Length ?? 0) > 0 && (lastNote?.Contains(newNote[0]) ?? false))
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
				*/
				if(newNote.Contains(C_NOTE) && currentOctave == 0)
				{
					currentOctave++;
				}
				noteList.Add($"{newNote}{currentOctave}");
				lastNote = newNote;
				currentNotesInScale++;
			}
			return noteList;
		}

		#endregion private helpers 
	}
}
