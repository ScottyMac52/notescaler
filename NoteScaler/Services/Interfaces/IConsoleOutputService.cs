namespace NoteScaler.Services.Interfaces
{
	using System;

	public interface IConsoleOutputService
	{
		void WriteMessage(string message, ConsoleColor textColor = ConsoleColor.White);
	}
}
