namespace NoteScaler.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;

	public sealed class StringInstrumentFactory : IStringInstrumentFactory
	{
		public IStringInstrument Create(TuningScheme tuningScheme, int numberOfFrets)
		{
			return new Guitar(tuningScheme, numberOfFrets);
		}

		public IStringInstrument Create(StringInstrumentDefinition definition)
		{
			return new Guitar(definition);
		}
	}
}
