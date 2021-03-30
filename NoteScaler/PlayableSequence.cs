namespace NoteScaler
{
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	public class PlayableSequence
	{
		public void Prepare()
		{
			NotePlayer = new NotePlayer();
			NoteSequence = PrepareSequence();
		}

		public void Play()
		{
			foreach (var songNoteDuration in NoteSequence)
			{
				try
				{
					songNoteDuration.PlayNote(songNoteDuration.Duration);
				}
				catch (Exception ex)
				{
					var currentForeground = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(ex.Message);
					Console.ForegroundColor = currentForeground;
				}
			}
		}

		public string[] Song { get; private set; }
		public IEnumerable<MusicNote> NoteSequence { get; private set; }
		public int MeasureTime { get; set; } = 1000;
		public int Octave { get; set; } = 2;
		public InstrumentType InstrumentType { get; set; } = InstrumentType.Horn;
		public int A4Reference { get; set; } = 440;
		public NotePlayer NotePlayer { get; private set;}

		
		public void LoadSequenceFromString(IEnumerable<string> sequence)
		{
			Song = sequence.ToArray();
			NoteSequence = PrepareSequence();
		}

		public bool LoadSequenceFromFile(string songName, string keyName = null)
		{
			if (!string.IsNullOrEmpty(songName))
			{
				var completeFileName = Path.Combine(Environment.CurrentDirectory, $"Songs\\{songName}.json");
				if (!File.Exists(completeFileName))
				{
					throw new FileNotFoundException($"Song file not found in {Path.Combine(Environment.CurrentDirectory, "Songs")}", songName);
				}
				var songDefinition = JsonConvert.DeserializeObject<Song>(File.ReadAllText(completeFileName));

				Song = songDefinition.Default;
				if (!string.IsNullOrEmpty(keyName) && songDefinition.Keys.Any(key => key.Name?.Equals(keyName, StringComparison.InvariantCultureIgnoreCase) ?? false))
				{
					Song = songDefinition.Keys.Where(key => key?.Name?.Equals(keyName, StringComparison.InvariantCultureIgnoreCase) ?? false)?.SelectMany(key => key.SongNotes).ToArray();
				}
				NoteSequence = PrepareSequence();
				return true;
			}

			return false;
		}

		private IEnumerable<MusicNote> PrepareSequence()
		{
			var currentInstrument = InstrumentType;
			return Song.Select(s => s.Split(',')[0]).Select(songDef =>
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
				return new MusicNote(currentNote, A4Reference, NotePlayer, GetNoteDuration(arry), currentInstrument);
			});
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
	}
}
