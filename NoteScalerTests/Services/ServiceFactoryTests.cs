namespace NoteScalerTests.Services
{
	using NoteScaler.Config;
	using NoteScaler.Enums;
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
	}
}
