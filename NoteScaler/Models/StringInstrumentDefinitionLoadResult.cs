namespace NoteScaler.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class StringInstrumentDefinitionLoadResult
	{
		private StringInstrumentDefinitionLoadResult(bool success, string error, IEnumerable<StringInstrumentDefinition> instruments)
		{
			Success = success;
			Error = error;
			Instruments = Array.AsReadOnly((instruments ?? Enumerable.Empty<StringInstrumentDefinition>()).ToArray());
		}

		public bool Success { get; }
		public string Error { get; }
		public IReadOnlyCollection<StringInstrumentDefinition> Instruments { get; }

		public static StringInstrumentDefinitionLoadResult Loaded(IEnumerable<StringInstrumentDefinition> instruments)
		{
			return new StringInstrumentDefinitionLoadResult(true, null, instruments);
		}

		public static StringInstrumentDefinitionLoadResult NotLoaded(string error)
		{
			return new StringInstrumentDefinitionLoadResult(false, error, null);
		}
	}
}
