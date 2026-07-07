namespace NoteScalerTests.Models
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;
	using System;

	public sealed class UnusedStringInstrumentFactory : IStringInstrumentFactory
	{
		public IStringInstrument Create(TuningScheme tuningScheme, int numberOfFrets)
		{
			throw new InvalidOperationException($"The string instrument factory should not be used by {nameof(UnusedStringInstrumentFactory)}.");
		}

		public IStringInstrument Create(string definitionName)
		{
			throw new InvalidOperationException($"The string instrument factory should not be used by {nameof(UnusedStringInstrumentFactory)}.");
		}

		public IStringInstrument Create(StringInstrumentDefinition definition)
		{
			throw new InvalidOperationException($"The string instrument factory should not be used by {nameof(UnusedStringInstrumentFactory)}.");
		}
	}
}
