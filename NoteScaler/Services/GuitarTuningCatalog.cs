namespace NoteScaler.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class GuitarTuningCatalog
	{
		public static IReadOnlyCollection<TuningDefinition> SupportedTunings => Enum
			.GetValues(typeof(TuningScheme))
			.Cast<TuningScheme>()
			.Select(GetDefinition)
			.ToArray();

		public static TuningDefinition GetDefinition(TuningScheme tuningScheme)
		{
			var definition = StringInstrumentCatalog.LoadBaseCatalog().GetDefinition(tuningScheme.ToString());
			return new TuningDefinition(tuningScheme, definition.GetOpenStringNotesByStringNumber());
		}
	}
}
