namespace NoteScaler.Services
{
	using BidirectionalMap;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MusicNote
	{
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
		private const string SHARP_NOTE = "#";
		private const string FLAT_NOTE = "b";
		private const int POWER_CHORD = 2;
		private const int MAJOR_MINOR_THREE_CHORD = 3;
		private const int MAJOR_MINOR_FIVE_CHORD = 5;

		private static readonly MusicNoteFrequencyCalculator FrequencyCalculator = new MusicNoteFrequencyCalculator();
		private static readonly MusicNoteScaleBuilder ScaleBuilder = new MusicNoteScaleBuilder();

		private bool isValid = false;
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

		public string Key { get; set; }
		public int Reference { get; set; }
		public ToneTypes ToneType { get; private set; }
		public IEnumerable<string> FlatNotes { get; private set; }
		public float[] Frequencies { get; set; }
		public float CurrentFrequency => Frequencies[DesiredOctave];
		public IEnumerable<string> SharpNotes { get; private set; }
		public int Duration { get; set; }
		public InstrumentType Instrument { get; set; }
		public bool IsNatural => !Key.Contains(SHARP_NOTE) && !Key.Contains(FLAT_NOTE);
		public bool IsFlat => Key.Contains(FLAT_NOTE);
		public bool IsSharp => Key.Contains(SHARP_NOTE);
		public int DesiredOctave => desiredOctave;
		public bool IsValid => isValid;
		public IPlayer NotePlayer { get; set; }
		public ChordType ChordType { get; private set; }
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

		public override string ToString()
		{
			return $"{Key}{DesiredOctave}-{Duration}ms";
		}

		public static MusicNote Create(string note, int a4Reference = 440, IPlayer player = null, int duration = 500, InstrumentType currentInstrument = default, ChordType chordType = ChordType.Note)
		{
			var musicNote = CheckCache(note, a4Reference);
			if (musicNote != null)
			{
				musicNote.Duration = duration;
				musicNote.Instrument = currentInstrument;
				musicNote.NotePlayer = player;
				musicNote.ChordType = chordType;
			}
			return musicNote;
		}

		private static MusicNote CheckCache(string note, int a4Reference)
		{
			MusicNote musicNote = null;
			if (MusicNoteCache == null)
			{
				MusicNoteCache = new BiMap<string, MusicNote>();
			}
			if (MusicNoteCache.Forward.ContainsKey(note))
			{
				musicNote = MusicNoteCache.Forward[note];
			}
			if (musicNote == null)
			{
				musicNote = new MusicNote()
				{
					Key = note,
					Reference = a4Reference
				};
				musicNote.Error += MusicNote_Error;
				musicNote.InitializeNote();
				if (musicNote.IsValid)
				{
					FactoryCreateNote?.Invoke(musicNote, new EventArgs());
					MusicNoteCache.Add(note, musicNote);
				}
				else
				{
					return null;
				}
			}
			return musicNote;
		}

		private static void MusicNote_Error(object sender, Exception e)
		{
			FactoryError?.Invoke(sender, e);
		}

		private MusicNote()
		{
			CreateNote?.Invoke(this, new EventArgs());
		}

		protected static BiMap<string, MusicNote> MusicNoteCache;

		public void PlayNote()
		{
			if (DesiredOctave > Frequencies.Count() - 1)
			{
				var error = new ArgumentException($"{DesiredOctave} is too high of an Octave to play {Key}", nameof(DesiredOctave));
				Error?.Invoke(this, error);
			}
			else
			{
				try
				{
					string[] desiredChord = null;
					switch (ChordType)
					{
						case ChordType.Note:
							desiredChord = new string[] { Key };
							break;
						case ChordType.Power:
							desiredChord = PowerChord;
							break;
						case ChordType.MinorThird:
							desiredChord = MinorChord3;
							break;
						case ChordType.MajorThird:
							desiredChord = MajorChord3;
							break;
						case ChordType.MinorSeventh:
							desiredChord = MinorChord7;
							break;
						case ChordType.MajorSeventh:
							desiredChord = MajorChord7;
							break;
					}

					PlayingNote?.Invoke(this, new EventArgs());
					var musicNotes = desiredChord.Select(sn => MusicNote.Create(sn));
					var frequencies = musicNotes.Select(mn => new FrequencyDuration(mn.Key, mn.DesiredOctave, mn.Frequencies[DesiredOctave], Duration));
					NotePlayer?.Play(frequencies, Instrument);
				}
				catch (Exception ex)
				{
					Error?.Invoke(this, ex);
				}
			}
		}

		public event EventHandler CreateNote;
		public event EventHandler PlayingNote;
		public event EventHandler<Exception> Error;
		public static event EventHandler<Exception> FactoryError;
		public static event EventHandler FactoryCreateNote;

		private void InitializeNote()
		{
			try
			{
				var token = NoteToken.Parse(Key);
				Key = token.Name;
				desiredOctave = token.Octave;
				ToneType = token.ToneType;

				var context = ScaleBuilder.GetNoteContext(Key);
				FlatNotes = context.FlatNotes;
				SharpNotes = context.SharpNotes;
				Frequencies = FrequencyCalculator.CalculateFrequencies(context.NoteIndex, Reference);
				InitializeScalesAndKeys(context);
				isValid = true;
			}
			catch (Exception ex)
			{
				Error?.Invoke(this, ex);
				isValid = false;
			}
		}

		private void InitializeScalesAndKeys(MusicNoteScaleContext context)
		{
			minorScale = ScaleBuilder.BuildMinorScale(Key, ToneType, context);
			majorScale = ScaleBuilder.BuildMajorScale(Key, ToneType, context);
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
			relativeMinorScale = ScaleBuilder.BuildRelativeMinorScale(majorScale, ToneType);
		}

		private string GetRelativeKey(bool forMajor = false)
		{
			return !forMajor ? MajorScale.ElementAt(RELATIVE_MINOR_POSITION) : MajorScale.ElementAt(RELATIVE_MAJOR_POSITION);
		}

		private string GetNoteBefore(bool useScale = true, bool useMinorScale = false)
		{
			var noteList = GetNoteList(useScale, useMinorScale);
			var currentPos = Array.FindIndex(noteList.ToArray(), nl => nl.Contains(Key));
			var targetPos = currentPos == 0 ? noteList.Count() - 2 : currentPos - 1;
			return noteList.ElementAt(targetPos);
		}

		private string GetNoteAfter(bool useScale = true, bool useMinorScale = false)
		{
			var noteList = GetNoteList(useScale, useMinorScale);
			var currentPos = Array.FindIndex(noteList.ToArray(), nl => nl.Contains(Key));
			var targetPos = currentPos == 12 ? 0 : currentPos + 1;
			return noteList.ElementAt(targetPos);
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
				noteList = IsNatural ? FlatNotes : IsSharp ? SharpNotes : FlatNotes;
			}

			return noteList;
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
