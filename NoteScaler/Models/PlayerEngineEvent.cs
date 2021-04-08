namespace NoteScaler.Models
{
	using NoteScaler.Enums;

	public class PlayerEngineEvent
	{
		public PlayerEventType EventType { get; set; }

		public string Message { get; set; }
	}
}