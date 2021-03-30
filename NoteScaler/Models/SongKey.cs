namespace NoteScaler.Models
{
	using Newtonsoft.Json;
	using System.Linq;

	public class SongKey
	{
		[JsonProperty("sequence")]
		public string Sequence { get; set; }

		[JsonProperty("keyName")]
		public string Name { get; set; }

		[JsonIgnore]
		public string[] SongNotes { get; protected set; }

		public SongKey(string name, string sequence)
		{
			Name = name;
			Sequence = sequence;
			InitializeSong();
		}

		private void InitializeSong()
		{
			SongNotes = Sequence.Split(',').Select(notes => notes.Trim()).ToArray();
		}

	}
}