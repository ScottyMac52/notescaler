namespace NoteScaler.Services
{
	using NoteScaler.Config;
	using NoteScaler.Enums;
	using NoteScaler.Services.Interfaces;

	public sealed class PlayableSequenceFactory : IPlayableSequenceFactory
	{
		private readonly IPlayerFactory playerFactory;
		private readonly IStringInstrumentFactory stringInstrumentFactory;
		private readonly IMusicNoteFactory musicNoteFactory;

		public PlayableSequenceFactory(IPlayerFactory playerFactory, IStringInstrumentFactory stringInstrumentFactory)
			: this(playerFactory, stringInstrumentFactory, new MusicNoteFactory(new MusicNoteCache(), new MusicNoteScaleBuilder(), new MusicNoteFrequencyCalculator()))
		{
		}

		public PlayableSequenceFactory(IPlayerFactory playerFactory, IStringInstrumentFactory stringInstrumentFactory, IMusicNoteFactory musicNoteFactory)
		{
			this.playerFactory = playerFactory;
			this.stringInstrumentFactory = stringInstrumentFactory;
			this.musicNoteFactory = musicNoteFactory;
		}

		public PlayableSequence Create(NoteScalerOptions options, int a4Reference)
		{
			return new PlayableSequence(
				playerFactory.Create,
				() => stringInstrumentFactory.Create(TuningScheme.Standard, 24),
				musicNoteFactory)
			{
				MeasureTime = options.Speed.GetValueOrDefault(),
				Octave = options.Octave.GetValueOrDefault(),
				A4Reference = a4Reference,
				InstrumentType = options.Instrument
			};
		}
	}
}
