namespace NoteScaler.Services.Interfaces
{
	using NoteScaler.Config;
	using NoteScaler.Services;

	public interface IPlayableSequenceFactory
	{
		PlayableSequence Create(NoteScalerOptions options, int a4Reference);
	}
}
