namespace NoteScaler.Models
{
	using Newtonsoft.Json;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class StringInstrumentDefinition
	{
		[JsonProperty("name", Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty("aliases")]
		public IEnumerable<string> Aliases { get; set; }

		[JsonProperty("strings", Required = Required.Always)]
		public int NumberOfStrings { get; set; }

		[JsonProperty("frets", Required = Required.Always)]
		public int Frets { get; set; }

		[JsonProperty("capo")]
		public int? Capo { get; set; }

		[JsonProperty("openStrings", Required = Required.Always)]
		public IEnumerable<StringInstrumentStringDefinition> OpenStrings { get; set; }

		public int EffectiveCapo => Capo.GetValueOrDefault();

		public IEnumerable<string> LookupNames => new[] { Name }
			.Concat(Aliases ?? Enumerable.Empty<string>())
			.Where(lookupName => !string.IsNullOrWhiteSpace(lookupName));

		public IEnumerable<string> GetOpenStringNotesByStringNumber()
		{
			return OpenStrings
				.OrderBy(stringDefinition => stringDefinition.Number)
				.Select(stringDefinition => stringDefinition.Note);
		}
	}
}
