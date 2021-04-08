namespace NoteScaler.Models
{
	public class FrequencyDuration
	{
		public FrequencyDuration(string note, int desiredOctave, float frequency, int duration)
		{
			Note = note;
			Octave = desiredOctave;
			Frequency = frequency;
			Duration = duration;
		}

		public string Note { get; }
		public int Octave { get; }
		public float Frequency { get; }
		public int Duration { get; }

		public override string ToString()
		{
			return $"{Note}{Octave}[{Duration}ms][{Frequency}Hz]";
		}
	}
}