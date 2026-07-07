namespace NoteScaler.Services
{
	using Newtonsoft.Json;
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	public sealed class StringInstrumentDefinitionLoader : IStringInstrumentDefinitionLoader
	{
		public StringInstrumentDefinitionLoadResult Load(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return StringInstrumentDefinitionLoadResult.NotLoaded("String instrument definition path is required.");
			}

			if (!File.Exists(path))
			{
				return StringInstrumentDefinitionLoadResult.NotLoaded($"String instrument definition file not found: {path}");
			}

			return LoadJson(path, File.ReadAllText(path));
		}

		public StringInstrumentDefinitionLoadResult LoadResource(Assembly assembly, string resourceName)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			using var resourceStream = assembly.GetManifestResourceStream(resourceName);
			if (resourceStream == null)
			{
				return StringInstrumentDefinitionLoadResult.NotLoaded($"String instrument definition resource not found: {resourceName}");
			}

			using var reader = new StreamReader(resourceStream);
			return LoadJson(resourceName, reader.ReadToEnd());
		}

		public StringInstrumentDefinitionLoadResult LoadJson(string sourceName, string json)
		{
			try
			{
				var document = JsonConvert.DeserializeObject<StringInstrumentDefinitionDocument>(json);
				var validationError = Validate(document);
				if (!string.IsNullOrEmpty(validationError))
				{
					return StringInstrumentDefinitionLoadResult.NotLoaded(validationError);
				}

				return StringInstrumentDefinitionLoadResult.Loaded(document.Instruments.ToArray());
			}
			catch (Exception ex)
			{
				return StringInstrumentDefinitionLoadResult.NotLoaded($"Unable to load string instrument definitions from {sourceName}: {ex.Message}");
			}
		}

		private static string Validate(StringInstrumentDefinitionDocument document)
		{
			var instruments = document?.Instruments?.ToArray();
			if (instruments == null || instruments.Length == 0)
			{
				return "String instrument definition file must contain at least one instrument.";
			}

			var duplicateName = instruments
				.Where(instrument => instrument != null)
				.SelectMany(instrument => instrument.LookupNames)
				.GroupBy(lookupName => lookupName, StringComparer.InvariantCultureIgnoreCase)
				.FirstOrDefault(group => group.Count() > 1);
			if (duplicateName != null)
			{
				return $"String instrument name or alias must be unique: {duplicateName.Key}.";
			}

			foreach (var instrument in instruments)
			{
				var validationError = ValidateInstrument(instrument);
				if (!string.IsNullOrEmpty(validationError))
				{
					return validationError;
				}
			}

			return null;
		}

		private static string ValidateInstrument(StringInstrumentDefinition instrument)
		{
			if (instrument == null)
			{
				return "String instrument definition cannot be null.";
			}

			if (string.IsNullOrWhiteSpace(instrument.Name))
			{
				return "String instrument name is required.";
			}

			if (instrument.NumberOfStrings <= 0)
			{
				return $"String instrument {instrument.Name} must define at least one string.";
			}

			if (instrument.Frets <= 0)
			{
				return $"String instrument {instrument.Name} must define at least one fret.";
			}

			if (instrument.EffectiveCapo < 0 || instrument.EffectiveCapo > instrument.Frets)
			{
				return $"String instrument {instrument.Name} has invalid capo location {instrument.EffectiveCapo}.";
			}

			var strings = instrument.OpenStrings?.ToArray();
			if (strings == null || strings.Length != instrument.NumberOfStrings)
			{
				return $"String instrument {instrument.Name} must define exactly {instrument.NumberOfStrings} open strings.";
			}

			if (strings.Any(stringDefinition => stringDefinition == null))
			{
				return $"String instrument {instrument.Name} has a null string definition.";
			}

			var duplicateString = strings
				.GroupBy(stringDefinition => stringDefinition.Number)
				.FirstOrDefault(group => group.Count() > 1);
			if (duplicateString != null)
			{
				return $"String instrument {instrument.Name} has duplicate string number {duplicateString.Key}.";
			}

			foreach (var stringDefinition in strings)
			{
				var validationError = ValidateString(instrument, stringDefinition);
				if (!string.IsNullOrEmpty(validationError))
				{
					return validationError;
				}
			}

			return null;
		}

		private static string ValidateString(StringInstrumentDefinition instrument, StringInstrumentStringDefinition stringDefinition)
		{
			if (stringDefinition.Number < 1 || stringDefinition.Number > instrument.NumberOfStrings)
			{
				return $"String instrument {instrument.Name} has invalid string number {stringDefinition.Number}.";
			}

			if (!PitchIndex.Default.Contains(stringDefinition.Note))
			{
				return $"String instrument {instrument.Name} string {stringDefinition.Number} has unsupported note {stringDefinition.Note}.";
			}

			try
			{
				PitchIndex.Default.GetNoteAbove(stringDefinition.Note, instrument.EffectiveCapo);
			}
			catch (ArgumentOutOfRangeException)
			{
				return $"String instrument {instrument.Name} string {stringDefinition.Number} capo note is outside the supported pitch range.";
			}

			return null;
		}
	}
}
