
namespace NoteScaler.Models
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;
	using NoteScaler.Enums;

	public class TabVersion
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("speed")]
		public int Speed { get; set; }

		[JsonProperty("tuning")]
		[JsonConverter(typeof(StringEnumConverter))]
		public TuningScheme Tuning { get; set; }

		[JsonProperty("tab")]
		public string TabString { get; set; }

	}
}