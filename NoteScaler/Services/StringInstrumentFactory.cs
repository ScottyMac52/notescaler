namespace NoteScaler.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;

	public sealed class StringInstrumentFactory : IStringInstrumentFactory
	{
		private readonly IStringInstrumentCatalog stringInstrumentCatalog;

		public StringInstrumentFactory()
			: this(StringInstrumentCatalog.LoadDefaultCatalog())
		{
		}

		public StringInstrumentFactory(IStringInstrumentCatalog stringInstrumentCatalog)
		{
			this.stringInstrumentCatalog = stringInstrumentCatalog;
		}

		public IStringInstrument Create(TuningScheme tuningScheme, int numberOfFrets)
		{
			return new Guitar(tuningScheme, numberOfFrets);
		}

		public IStringInstrument Create(string definitionName)
		{
			return Create(stringInstrumentCatalog.GetDefinition(definitionName));
		}

		public IStringInstrument Create(StringInstrumentDefinition definition)
		{
			return new Guitar(definition);
		}
	}
}
