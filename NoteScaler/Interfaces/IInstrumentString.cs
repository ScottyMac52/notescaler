namespace NoteScaler.Interfaces
{
	using System.Collections.Generic;

	public interface IInstrumentString
	{
		/// <summary>
		/// Max number of Frets
		/// </summary>
		int MaxFret { get; }
		
		/// <summary>
		/// This string number
		/// </summary>
		int Number { get; }
		
		/// <summary>
		/// Tuned Note
		/// </summary>
		string Tuning { get; }

		/// <summary>
		/// Gets a note by Fret
		/// </summary>
		/// <param name="fret"></param>
		/// <returns></returns>
		string GetNote(int fret);
		
		/// <summary>
		/// Gets the Fret by note
		/// </summary>
		/// <param name="note"></param>
		/// <returns></returns>
		int GetNote(string note);

		/// <summary>
		/// Gets all the notes in the string
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetNotesInString();

		/// <summary>
		/// Sets the Tuning of the String
		/// </summary>
		/// <param name="noteToTune"></param>
		void SetTuning(string noteToTune);
	}
}