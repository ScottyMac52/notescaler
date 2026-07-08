namespace NoteScaler.Models
{
	using Newtonsoft.Json;

	public sealed class GtabDocument
	{
		[JsonProperty("schemaVersion", Required = Required.Always)]
		public int SchemaVersion { get; set; }

		[JsonProperty("name", Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty("speed")]
		public int Speed { get; set; }

		[JsonProperty("tuning", Required = Required.Always)]
		public string Tuning { get; set; }

		[JsonProperty("tab", Required = Required.Always)]
		public string TabString { get; set; }

		[JsonProperty("repeat")]
		public int? Repeat { get; set; } = 1;

		[JsonProperty("strings")]
		public int NumberOfStrings { get; set; } = 6;
	}
}
