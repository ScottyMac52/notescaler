namespace NoteScaler
{
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

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
