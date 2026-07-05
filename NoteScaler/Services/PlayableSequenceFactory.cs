namespace NoteScaler.Services
{
	using NoteScaler.Classes;
	using NoteScaler.Config;
	using NoteScaler.Enums;
	using NoteScaler.Services.Interfaces;

	public sealed class PlayableSequenceFactory : IPlayableSequenceFactory
	{
		private readonly IPlayerFactory playerFactory;
		private readonly IStringInstrumentFactory stringInstrumentFactory;

		public PlayableSequenceFactory(IPlayerFactory playerFactory, IStringInstrumentFactory stringInstrumentFactory)
		{
			this.playerFactory = playerFactory;
			this.stringInstrumentFactory = stringInstrumentFactory;
		}

		public PlayableSequence Create(NoteScalerOptions options, int a4Reference)
		{
			return new PlayableSequence(
				playerFactory.Create,
				() => stringInstrumentFactory.Create(TuningScheme.Standard, 24))
			{
				MeasureTime = options.Speed.GetValueOrDefault(),
				Octave = options.Octave.GetValueOrDefault(),
				A4Reference = a4Reference,
				InstrumentType = options.Instrument
			};
		}
	}
}
