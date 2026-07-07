namespace NoteScaler.Services.Interfaces
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;

	public interface IStringInstrumentFactory
	{
		IStringInstrument Create(TuningScheme tuningScheme, int numberOfFrets);
		IStringInstrument Create(string definitionName);
		IStringInstrument Create(StringInstrumentDefinition definition);
	}
}
