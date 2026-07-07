namespace NoteScaler.Models
{
	using Newtonsoft.Json;
	using System.Collections.Generic;

	public sealed class StringInstrumentDefinitionDocument
	{
		[JsonProperty("instruments", Required = Required.Always)]
		public IEnumerable<StringInstrumentDefinition> Instruments { get; set; }
	}
}
