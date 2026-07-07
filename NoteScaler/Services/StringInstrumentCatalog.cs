namespace NoteScaler.Services
{
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	public sealed class StringInstrumentCatalog : IStringInstrumentCatalog
	{
		public const string BaseResourceName = "NoteScaler.Instruments.base-string-instruments.json";
		public const string UserDefinitionsRelativePath = "Instruments/string-instruments.json";

		private readonly IReadOnlyDictionary<string, StringInstrumentDefinition> definitionsByName;

		public StringInstrumentCatalog(IEnumerable<StringInstrumentDefinition> baseDefinitions, IEnumerable<StringInstrumentDefinition> userDefinitions = null)
		{
			var definitions = CombineDefinitions(baseDefinitions, userDefinitions).ToArray();
			Definitions = Array.AsReadOnly(definitions);
			definitionsByName = CreateLookup(definitions);
		}

		public IReadOnlyCollection<StringInstrumentDefinition> Definitions { get; }

		public static StringInstrumentCatalog LoadBaseCatalog()
		{
			var loader = new StringInstrumentDefinitionLoader();
			var baseDefinitions = LoadRequiredBaseDefinitions(loader);
			return new StringInstrumentCatalog(baseDefinitions);
		}

		public static StringInstrumentCatalog LoadDefaultCatalog()
		{
			var loader = new StringInstrumentDefinitionLoader();
			var baseDefinitions = LoadRequiredBaseDefinitions(loader);
			var userDefinitions = LoadUserDefinitionsIfAvailable(loader, GetUserDefinitionsPath());
			return new StringInstrumentCatalog(baseDefinitions, userDefinitions);
		}

		public bool TryGetDefinition(string name, out StringInstrumentDefinition definition)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				definition = null;
				return false;
			}

			return definitionsByName.TryGetValue(name, out definition);
		}

		public StringInstrumentDefinition GetDefinition(string name)
		{
			if (TryGetDefinition(name, out var definition))
			{
				return definition;
			}

			throw new ArgumentException($"Unsupported string instrument: {name}", nameof(name));
		}

		private static IReadOnlyCollection<StringInstrumentDefinition> LoadRequiredBaseDefinitions(StringInstrumentDefinitionLoader loader)
		{
			var result = loader.LoadResource(Assembly.GetExecutingAssembly(), BaseResourceName);
			if (!result.Success)
			{
				throw new InvalidOperationException(result.Error);
			}

			return result.Instruments;
		}

		private static IReadOnlyCollection<StringInstrumentDefinition> LoadUserDefinitionsIfAvailable(StringInstrumentDefinitionLoader loader, string path)
		{
			if (!File.Exists(path))
			{
				return Array.Empty<StringInstrumentDefinition>();
			}

			var result = loader.Load(path);
			return result.Success ? result.Instruments : Array.Empty<StringInstrumentDefinition>();
		}

		private static string GetUserDefinitionsPath()
		{
			var currentDirectoryPath = Path.Combine(Environment.CurrentDirectory, UserDefinitionsRelativePath);
			if (File.Exists(currentDirectoryPath))
			{
				return currentDirectoryPath;
			}

			return Path.Combine(AppContext.BaseDirectory, UserDefinitionsRelativePath);
		}

		private static IEnumerable<StringInstrumentDefinition> CombineDefinitions(IEnumerable<StringInstrumentDefinition> baseDefinitions, IEnumerable<StringInstrumentDefinition> userDefinitions)
		{
			var combined = new List<StringInstrumentDefinition>();
			var lookupNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

			foreach (var definition in baseDefinitions ?? Enumerable.Empty<StringInstrumentDefinition>())
			{
				combined.Add(definition);
				foreach (var lookupName in definition.LookupNames)
				{
					lookupNames.Add(lookupName);
				}
			}

			foreach (var definition in userDefinitions ?? Enumerable.Empty<StringInstrumentDefinition>())
			{
				if (definition.LookupNames.Any(lookupNames.Contains))
				{
					continue;
				}

				combined.Add(definition);
				foreach (var lookupName in definition.LookupNames)
				{
					lookupNames.Add(lookupName);
				}
			}

			return combined;
		}

		private static IReadOnlyDictionary<string, StringInstrumentDefinition> CreateLookup(IEnumerable<StringInstrumentDefinition> definitions)
		{
			return definitions
				.SelectMany(definition => definition.LookupNames.Select(lookupName => new { lookupName, definition }))
				.ToDictionary(item => item.lookupName, item => item.definition, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
