namespace NoteScaler.Services.Interfaces
{
	using NoteScaler.Models;
	using System.Collections.Generic;

	public interface IStringInstrumentCatalog
	{
		IReadOnlyCollection<StringInstrumentDefinition> Definitions { get; }
		bool TryGetDefinition(string name, out StringInstrumentDefinition definition);
		StringInstrumentDefinition GetDefinition(string name);
	}
}
