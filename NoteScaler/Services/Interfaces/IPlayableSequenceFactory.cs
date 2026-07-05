namespace NoteScaler.Services.Interfaces
{
	using NoteScaler.Config;

	public interface IPlayableSequenceFactory
	{
		PlayableSequence Create(NoteScalerOptions options, int a4Reference);
	}
}
