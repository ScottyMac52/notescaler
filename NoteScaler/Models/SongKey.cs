namespace NoteScaler.Models
{
	using Newtonsoft.Json;
	using NoteScaler.Enums;
	using System.Linq;

	public class SongKey
	{
		[JsonProperty("sequence")]
		public string Sequence { get; set; }

		[JsonProperty("keyName", Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty("chordType", DefaultValueHandling = DefaultValueHandling.Populate)]
		public ChordType ChordType { get; set; } = ChordType.Note;

		[JsonIgnore]
		public string[] SongNotes { get; protected set; }

		public SongKey(string name, string sequence)
		{
			Name = name;
			Sequence = sequence;
			InitializeSong();
		}

		public void SetNoteSequence(string[] noteSequence)
		{
			SongNotes = noteSequence;
		}

		private void InitializeSong()
		{
			SongNotes = Sequence?.Split(',')?.Select(notes => notes.Trim())?.ToArray();
		}

	}
}