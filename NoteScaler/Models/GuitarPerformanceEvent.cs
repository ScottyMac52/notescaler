namespace NoteScaler.Models
{
	using NoteScaler.Enums;

	public sealed class GuitarPerformanceEvent
	{
		public GuitarPerformanceEvent(
			string note,
			int stringNumber,
			int fret,
			int startOffsetMilliseconds,
			int durationMilliseconds,
			int velocity = 100,
			GuitarArticulation articulation = GuitarArticulation.Pick)
		{
			Note = note;
			StringNumber = stringNumber;
			Fret = fret;
			StartOffsetMilliseconds = startOffsetMilliseconds;
			DurationMilliseconds = durationMilliseconds;
			Velocity = velocity;
			Articulation = articulation;
		}

		public string Note { get; }
		public int StringNumber { get; }
		public int Fret { get; }
		public int StartOffsetMilliseconds { get; }
		public int DurationMilliseconds { get; }
		public int Velocity { get; }
		public GuitarArticulation Articulation { get; }
	}
}
