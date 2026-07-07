namespace NoteScaler.Models
{
	using Newtonsoft.Json;

	public class TabVersion
	{
		[JsonProperty("name", Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty("speed")]
		public int Speed { get; set; }

		[JsonProperty("tuning", Required = Required.Always)]
		public string Tuning { get; set; }

		[JsonProperty("tab", Required = Required.Always)]
		public string TabString { get; set; }

	}
}
