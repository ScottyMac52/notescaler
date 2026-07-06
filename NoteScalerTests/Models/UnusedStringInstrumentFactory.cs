namespace NoteScalerTests.Models
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Services.Interfaces;
	using System;

	public sealed class UnusedStringInstrumentFactory : IStringInstrumentFactory
	{
		public IStringInstrument Create(TuningScheme tuningScheme, int numberOfFrets)
		{
			throw new InvalidOperationException($"The string instrument factory should not be used by {nameof(UnusedStringInstrumentFactory)}.");
		}
	}
}
