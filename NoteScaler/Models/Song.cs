namespace NoteScaler.Models
{
	using Newtonsoft.Json;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	[ExcludeFromCodeCoverage]
	public class Song : SongKey
	{
		[JsonProperty("reverse")]
		public bool Reverse { get; set; } = false;

		[JsonProperty("keys")]
		public IEnumerable<SongKey> Keys {get;set;}

		[JsonIgnore]
		public string[] Default => SongNotes;

		public Song(string songName, string sequence) : base(songName, sequence)
		{
		}
	}
}
