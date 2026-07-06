namespace NoteScaler.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Services.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MusicNote
	{
		private const int POWER_CHORD = 2;
		private const int MAJOR_MINOR_THREE_CHORD = 3;
		private const int MAJOR_MINOR_FIVE_CHORD = 5;
		private const string SHARP_NOTE = "#";
		private const string FLAT_NOTE = "b";

		private static readonly IMusicNoteFactory DefaultFactory = new MusicNoteFactory(new MusicNoteCache(), new MusicNoteScaleBuilder(), new MusicNoteFrequencyCalculator());
		private static readonly IMusicNotePlayer DefaultPlayer = new MusicNotePlayer(DefaultFactory, new MusicNoteChordSelector());

		private readonly bool isValid;
		private readonly int desiredOctave;
		private readonly IEnumerable<string> minorScale;
		private readonly IEnumerable<string> relativeMinorScale;
		private readonly IEnumerable<string> majorScale;
		private readonly string relativeMinor;
		private readonly string relativeMajor;
		private readonly string noteBefore;
		private readonly string noteAfter;
		private readonly string minorNoteBefore;
		private readonly string minorNoteAfter;
		private readonly string majorNoteBefore;
		private readonly string majorNoteAfter;
		private readonly string[] majorChord;
		private readonly string[] minorChord;

		internal MusicNote(
			string key,
			int reference,
			ToneTypes toneType,
			int desiredOctave,
			IEnumerable<string> flatNotes,
			IEnumerable<string> sharpNotes,
			float[] frequencies,
			IEnumerable<string> minorScale,
			IEnumerable<string> majorScale,
			IEnumerable<string> relativeMinorScale,
			string relativeMinor,
			string relativeMajor,
			string noteBefore,
			string noteAfter,
			string minorNoteBefore,
			string minorNoteAfter,
			string majorNoteBefore,
			string majorNoteAfter,
			string[] majorChord,
			string[] minorChord)
		{
			Key = key;
			Reference = reference;
			ToneType = toneType;
			this.desiredOctave = desiredOctave;
			FlatNotes = flatNotes;
			SharpNotes = sharpNotes;
			Frequencies = frequencies;
			this.minorScale = minorScale;
			this.majorScale = majorScale;
			this.relativeMinorScale = relativeMinorScale;
			this.relativeMinor = relativeMinor;
			this.relativeMajor = relativeMajor;
			this.noteBefore = noteBefore;
			this.noteAfter = noteAfter;
			this.minorNoteBefore = minorNoteBefore;
			this.minorNoteAfter = minorNoteAfter;
			this.majorNoteBefore = majorNoteBefore;
			this.majorNoteAfter = majorNoteAfter;
			this.majorChord = majorChord;
			this.minorChord = minorChord;
			isValid = true;
		}

		public string Key { get; }
		public int Reference { get; }
		public ToneTypes ToneType { get; }
		public IEnumerable<string> FlatNotes { get; }
		public float[] Frequencies { get; }
		public float CurrentFrequency => Frequencies[DesiredOctave];
		public IEnumerable<string> SharpNotes { get; }
		public int Duration { get; set; }
		public InstrumentType Instrument { get; set; }
		public bool IsNatural => !Key.Contains(SHARP_NOTE) && !Key.Contains(FLAT_NOTE);
		public bool IsFlat => Key.Contains(FLAT_NOTE);
		public bool IsSharp => Key.Contains(SHARP_NOTE);
		public int DesiredOctave => desiredOctave;
		public bool IsValid => isValid;
		public IPlayer NotePlayer { get; set; }
		public ChordType ChordType { get; internal set; }
		public string RelativeMinor => relativeMinor;
		public string RelativeMajor => relativeMajor;
		public IEnumerable<string> MajorScale => majorScale;
		public IEnumerable<string> MinorScale => minorScale;
		public IEnumerable<string> RelativeMinorScale => relativeMinorScale;
		public string NoteBefore => noteBefore;
		public string NoteAfter => noteAfter;
		public string MajorNoteBefore => majorNoteBefore;
		public string MajorNoteAfter => majorNoteAfter;
		public string MinorNoteBefore => minorNoteBefore;
		public string MinorNoteAfter => minorNoteAfter;
		public string[] MajorChord15 => majorChord;
		public string[] MinorChord15 => minorChord;
		public string[] PowerChord => GetChord(POWER_CHORD);
		public string[] MajorChord3 => GetChord(MAJOR_MINOR_THREE_CHORD, true);
		public string[] MinorChord3 => GetChord(MAJOR_MINOR_THREE_CHORD);
		public string[] MinorChord7 => GetChord(MAJOR_MINOR_FIVE_CHORD);
		public string[] MajorChord7 => GetChord(MAJOR_MINOR_FIVE_CHORD, true);

		public event EventHandler CreateNote;
		public event EventHandler PlayingNote;
		public event EventHandler<Exception> Error;
		public static event EventHandler<Exception> FactoryError;
		public static event EventHandler FactoryCreateNote;

		public override string ToString()
		{
			return $"{Key}{DesiredOctave}-{Duration}ms";
		}

		public static MusicNote Create(string note, int a4Reference = 440, IPlayer player = null, int duration = 500, InstrumentType currentInstrument = default, ChordType chordType = ChordType.Note)
		{
			return DefaultFactory.Create(note, a4Reference, player, duration, currentInstrument, chordType);
		}

		public void PlayNote()
		{
			DefaultPlayer.Play(this);
		}

		internal void RaisePlayingNote()
		{
			PlayingNote?.Invoke(this, new EventArgs());
		}

		internal void RaiseError(Exception exception)
		{
			Error?.Invoke(this, exception);
		}

		internal static void RaiseFactoryCreateNote(MusicNote musicNote)
		{
			FactoryCreateNote?.Invoke(musicNote, new EventArgs());
		}

		internal static void RaiseFactoryError(object sender, Exception exception)
		{
			FactoryError?.Invoke(sender, exception);
		}

		private string[] GetChord(int degrees, bool isMajor = false)
		{
			var noteList = new List<string>();

			if (degrees == POWER_CHORD)
			{
				noteList.Add(majorChord[0]);
				noteList.Add(majorChord[2]);
			}
			else if (isMajor)
			{
				noteList.AddRange(majorChord.Take(degrees));
			}
			else
			{
				noteList.AddRange(minorChord.Take(degrees));
			}

			return noteList.ToArray();
		}
	}
}
