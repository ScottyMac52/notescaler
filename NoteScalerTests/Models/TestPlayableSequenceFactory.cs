namespace NoteScalerTests.Models
{
	using NoteScaler.Config;
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using NoteScaler.Services.Interfaces;

	public sealed class TestPlayableSequenceFactory : IPlayableSequenceFactory
	{
		public PlayableSequence Create(NoteScalerOptions options, int a4Reference)
		{
			return new PlayableSequence(() => new NoOpPlayer(), () => new Guitar())
			{
				MeasureTime = options.Speed.GetValueOrDefault(),
				Octave = options.Octave.GetValueOrDefault(),
				A4Reference = a4Reference,
				InstrumentType = options.Instrument
			};
		}
	}
}
