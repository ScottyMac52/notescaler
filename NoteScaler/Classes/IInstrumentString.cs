using System.Collections.Generic;

namespace NoteScaler.Classes
{
	public interface IInstrumentString
	{
		int MaxFret { get; }
		int Number { get; }
		string Tuning { get; }

		string GetNote(int fret);
		int GetNote(string note);
		IEnumerable<string> GetNotesInString();
		void SetTuning(string noteToTune);
		string ToString();
	}
}