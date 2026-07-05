namespace NoteScaler.Services
{
	using NoteScaler.Services.Interfaces;
	using System;

	public sealed class ConsoleOutputService : IConsoleOutputService
	{
		public void WriteMessage(string message, ConsoleColor textColor = ConsoleColor.White)
		{
			var currentForeground = Console.ForegroundColor;
			Console.ForegroundColor = textColor;
			Console.WriteLine(message);
			Console.ForegroundColor = currentForeground;
		}
	}
}
