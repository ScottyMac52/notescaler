namespace NoteScaler.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class GuitarTuningCatalog
	{
		private static readonly IReadOnlyDictionary<TuningScheme, TuningDefinition> Definitions = new[]
		{
			new TuningDefinition(TuningScheme.Standard, new[] { "E4", "B3", "G3", "D3", "A2", "E2" }),
			new TuningDefinition(TuningScheme.DropC, new[] { "D4", "A3", "F3", "C3", "G2", "C2" }),
			new TuningDefinition(TuningScheme.DropCSharp, new[] { "D#4", "A#3", "F#3", "C#3", "G#2", "C#2" }),
			new TuningDefinition(TuningScheme.DropD, new[] { "D4", "B3", "G3", "D3", "A2", "D2" }),
			new TuningDefinition(TuningScheme.OpenC, new[] { "E4", "C3", "G3", "C3", "G2", "C2" }),
			new TuningDefinition(TuningScheme.OpenD, new[] { "D4", "A3", "F#3", "D3", "A2", "D2" }),
			new TuningDefinition(TuningScheme.DropB, new[] { "C#4", "G#3", "E3", "B2", "F#2", "B1" }),
			new TuningDefinition(TuningScheme.DStandard, new[] { "D4", "A3", "F3", "C3", "G2", "D2" }),
			new TuningDefinition(TuningScheme.EbStandard, new[] { "D#4", "A#3", "F#3", "C#3", "G#2", "D#2" }),
			new TuningDefinition(TuningScheme.CSharpStandard, new[] { "C#4", "G#3", "E3", "B2", "F#2", "C#2" }),
			new TuningDefinition(TuningScheme.DADGAD, new[] { "D4", "A3", "G3", "D3", "A2", "D2" }),
			new TuningDefinition(TuningScheme.OpenG, new[] { "D4", "B3", "G3", "D3", "G2", "D2" })
		}.ToDictionary(definition => definition.TuningScheme);

		public static IReadOnlyCollection<TuningDefinition> SupportedTunings => Definitions.Values.ToArray();

		public static TuningDefinition GetDefinition(TuningScheme tuningScheme)
		{
			if (Definitions.TryGetValue(tuningScheme, out var definition))
			{
				return definition;
			}

			throw new ArgumentException($"Unsupported tuning scheme: {tuningScheme}", nameof(tuningScheme));
		}
	}
}
