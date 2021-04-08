namespace NoteScaler.Models
{
	using NoteScaler.Enums;

	public class PlayableSequenceEvent 
	{
		public PlayableEventType EventType { get; set; }

		public string EventDetails { get; set; }
	}
}