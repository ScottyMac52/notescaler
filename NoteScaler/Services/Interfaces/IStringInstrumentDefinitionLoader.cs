namespace NoteScaler.Services.Interfaces
{
	using NoteScaler.Models;

	public interface IStringInstrumentDefinitionLoader
	{
		StringInstrumentDefinitionLoadResult Load(string path);
	}
}
