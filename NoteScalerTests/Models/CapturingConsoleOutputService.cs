namespace NoteScalerTests.Models
{
	using NoteScaler.Services.Interfaces;
	using System;
	using System.Collections.Generic;

	public sealed class CapturingConsoleOutputService : IConsoleOutputService
	{
		public List<string> Messages { get; } = new List<string>();

		public void WriteMessage(string message, ConsoleColor textColor = ConsoleColor.White)
		{
			Messages.Add(message);
		}
	}
}
