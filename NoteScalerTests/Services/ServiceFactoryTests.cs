namespace NoteScalerTests.Services
{
	using NoteScaler.Config;
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using System.Linq;
	using Xunit;

	public class ServiceFactoryTests
	{
		[Fact]
		public void CommandLineOptionsService_PreservesExistingCommandLineParsing()
		{
			var service = new CommandLineOptionsService();

			var parsedOptions = service.ParseArguments(new[] { "--note", "C" });

			Assert.Equal("Parsed", parsedOptions.Tag.ToString());
		}

		[Fact]
		public void PlayableSequenceFactory_CreatesConfiguredPlayableSequence()
		{
			var factory = new PlayableSequenceFactory(new SignalNotePlayerFactory(), new StringInstrumentFactory());
			var options = new NoteScalerOptions
			{
				Speed = 300,
				Octave = 3,
				Instrument = InstrumentType.Horn
			};

			var playableSequence = factory.Create(options, 440);

			Assert.Equal(300, playableSequence.MeasureTime);
			Assert.Equal(3, playableSequence.Octave);
			Assert.Equal(440, playableSequence.A4Reference);
			Assert.Equal(InstrumentType.Horn, playableSequence.InstrumentType);
		}

		[Fact]
		public void StringInstrumentFactory_CreatesGuitarWithRequestedTuningAndFrets()
		{
			var factory = new StringInstrumentFactory();

			var instrument = factory.Create(TuningScheme.DropD, 21);

			var guitar = Assert.IsType<Guitar>(instrument);
			Assert.Equal(TuningScheme.DropD, guitar.TuningScheme);
			Assert.Equal(21, guitar.Frets);
			Assert.Equal(6, guitar.Strings.Count());
		}

		[Fact]
		public void StringInstrumentFactory_CreatesGuitarFromCustomDefinition()
		{
			var factory = new StringInstrumentFactory();
			var definition = new StringInstrumentDefinition
			{
				Name = "Four String Bass",
				NumberOfStrings = 4,
				Frets = 20,
				Capo = 1,
				OpenStrings = new[]
				{
					new StringInstrumentStringDefinition { Number = 1, Note = "G2" },
					new StringInstrumentStringDefinition { Number = 2, Note = "D2" },
					new StringInstrumentStringDefinition { Number = 3, Note = "A1" },
					new StringInstrumentStringDefinition { Number = 4, Note = "E1" }
				}
			};

			var instrument = factory.Create(definition);

			var guitar = Assert.IsType<Guitar>(instrument);
			Assert.Equal("Four String Bass", guitar.Name);
			Assert.Equal(20, guitar.Frets);
			Assert.Equal(1, guitar.Capo);
			Assert.Equal(4, guitar.Strings.Count());
			Assert.Equal("G#2", guitar.GetNote(1, 0));
			Assert.Equal("F1", guitar.GetNote(4, 0));
		}
	}
}
