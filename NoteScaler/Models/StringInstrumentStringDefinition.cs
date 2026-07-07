namespace NoteScaler.Models
{
	using Newtonsoft.Json;

	public sealed class StringInstrumentStringDefinition
	{
		[JsonProperty("number", Required = Required.Always)]
		public int Number { get; set; }

		[JsonProperty("note", Required = Required.Always)]
		public string Note { get; set; }
	}
}
