namespace NoteScaler.Models
{
	using Newtonsoft.Json;
	using System.Collections.Generic;

	public class Song : SongKey
	{
		[JsonProperty("keys")]
		public IEnumerable<SongKey> Keys {get;set;}

		[JsonIgnore]
		public string[] Default => SongNotes;

		public Song(string songName, string sequence) : base(songName, sequence)
		{
		}
	}
}
