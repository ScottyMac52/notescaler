namespace NoteScaler.Services.Interfaces
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;

	public interface IStringInstrumentFactory
	{
		IStringInstrument Create(TuningScheme tuningScheme, int numberOfFrets);
	}
}
