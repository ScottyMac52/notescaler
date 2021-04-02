namespace NoteScaler.Models
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;
	using NoteScaler.Enums;

	public class TabVersion
	{
		[JsonProperty("name", Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty("speed")]
		public int Speed { get; set; }

		[JsonProperty("tuning", Required = Required.Always)]
		[JsonConverter(typeof(StringEnumConverter))]
		public TuningScheme Tuning { get; set; }

		[JsonProperty("tab", Required = Required.Always)]
		public string TabString { get; set; }

	}
}