namespace NoteScalerTests.Services
{
	using NoteScaler.Models;
	using NoteScaler.Services;
	using System.Linq;
	using Xunit;

	public class StringInstrumentCatalogTests
	{
		[Fact]
		public void LoadBaseCatalog_LoadsEmbeddedBaseDefinitions()
		{
			var catalog = StringInstrumentCatalog.LoadBaseCatalog();

			Assert.True(catalog.Definitions.Count >= 13);
			Assert.True(catalog.TryGetDefinition("Standard", out _));
			Assert.True(catalog.TryGetDefinition("Eb Standard", out _));
			Assert.True(catalog.TryGetDefinition("Drop D", out _));
			Assert.True(catalog.TryGetDefinition("Drop C#", out _));
			Assert.True(catalog.TryGetDefinition("Drop C", out _));
			Assert.True(catalog.TryGetDefinition("Drop B", out _));
			Assert.True(catalog.TryGetDefinition("Drop A", out _));
			Assert.True(catalog.TryGetDefinition("DADGAD", out _));
			Assert.True(catalog.TryGetDefinition("Open A", out _));
			Assert.True(catalog.TryGetDefinition("Open G", out _));
			Assert.True(catalog.TryGetDefinition("Ukulele", out _));
			Assert.True(catalog.TryGetDefinition("12 String Standard", out _));
			Assert.True(catalog.TryGetDefinition("7 String Drop A", out _));
		}

		[Fact]
		public void LoadBaseCatalog_SupportsLegacyTuningSchemeAliases()
		{
			var catalog = StringInstrumentCatalog.LoadBaseCatalog();

			Assert.True(catalog.TryGetDefinition("DropD", out var dropD));
			Assert.Equal("Drop D", dropD.Name);
			Assert.True(catalog.TryGetDefinition("DropCSharp", out var dropCSharp));
			Assert.Equal("Drop C#", dropCSharp.Name);
			Assert.True(catalog.TryGetDefinition("OpenG", out var openG));
			Assert.Equal("Open G", openG.Name);
		}

		[Fact]
		public void Constructor_SupplementsBaseDefinitionsWithUserDefinitions()
		{
			var catalog = new StringInstrumentCatalog(
				new[] { CreateDefinition("Standard", 6, "E4", "B3", "G3", "D3", "A2", "E2") },
				new[] { CreateDefinition("Mandolin", 4, "E5", "A4", "D4", "G3") });

			Assert.True(catalog.TryGetDefinition("Standard", out _));
			Assert.True(catalog.TryGetDefinition("Mandolin", out var mandolin));
			Assert.Equal(4, mandolin.NumberOfStrings);
		}

		[Fact]
		public void Constructor_DoesNotAllowUserDefinitionsToOverrideBaseDefinitions()
		{
			var catalog = new StringInstrumentCatalog(
				new[] { CreateDefinition("Standard", 6, "E4", "B3", "G3", "D3", "A2", "E2") },
				new[] { CreateDefinition("Standard", 4, "E5", "A4", "D4", "G3") });

			var definition = catalog.GetDefinition("Standard");

			Assert.Equal(6, definition.NumberOfStrings);
			Assert.Equal(new[] { "E4", "B3", "G3", "D3", "A2", "E2" }, definition.GetOpenStringNotesByStringNumber());
		}

		private static StringInstrumentDefinition CreateDefinition(string name, int numberOfStrings, params string[] notes)
		{
			return new StringInstrumentDefinition
			{
				Name = name,
				NumberOfStrings = numberOfStrings,
				Frets = 24,
				OpenStrings = notes.Select((note, index) => new StringInstrumentStringDefinition
				{
					Number = index + 1,
					Note = note
				}).ToArray()
			};
		}
	}
}
