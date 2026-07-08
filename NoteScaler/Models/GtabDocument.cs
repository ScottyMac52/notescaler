namespace NoteScaler.Models
{
	using Newtonsoft.Json;
	using System.Collections.Generic;

	public sealed class GtabDocument
	{
		[JsonProperty("cFret")]
		public int CapoFret { get; set; }

		[JsonProperty("title", Required = Required.Always)]
		public string Title { get; set; }

		[JsonProperty("tempo")]
		public int Tempo { get; set; }

		[JsonProperty("stringNotes", Required = Required.Always)]
		public IEnumerable<string> StringNotes { get; set; }

		[JsonProperty("version")]
		public int Version { get; set; }

		[JsonProperty("lyricSize")]
		public int LyricSize { get; set; }

		[JsonProperty("tabRows", Required = Required.Always)]
		public IEnumerable<GtabRow> TabRows { get; set; }
	}

	public sealed class GtabRow
	{
		[JsonProperty("lyricLines")]
		public IEnumerable<string> LyricLines { get; set; }

		[JsonProperty("lyrics")]
		public string Lyrics { get; set; }

		[JsonProperty("columnHeaders")]
		public IEnumerable<GtabColumnHeader> ColumnHeaders { get; set; }

		[JsonProperty("columns", Required = Required.Always)]
		public IEnumerable<IEnumerable<GtabCell>> Columns { get; set; }
	}

	public sealed class GtabColumnHeader
	{
		[JsonProperty("name")]
		public int Name { get; set; }

		[JsonProperty("strum")]
		public int Strum { get; set; }
	}

	public sealed class GtabCell
	{
		[JsonProperty("p")]
		public string Position { get; set; }

		[JsonProperty("s")]
		public string Style { get; set; }
	}
}
