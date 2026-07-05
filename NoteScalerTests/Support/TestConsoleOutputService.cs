namespace NoteScalerTests.Support
{
	using NoteScaler.Services.Interfaces;
	using System;
	using System.Collections.Generic;

	public sealed class TestConsoleOutputService : IConsoleOutputService
	{
		private readonly List<string> messages = new List<string>();

		public IReadOnlyList<string> Messages => messages;

		public void WriteMessage(string message, ConsoleColor textColor = ConsoleColor.White)
		{
			messages.Add(message);
		}
	}
}
