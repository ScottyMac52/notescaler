namespace NoteScaler.Services
{
	using NoteScaler.Classes;
	using NoteScaler.Interfaces;
	using NoteScaler.Services.Interfaces;

	public sealed class SignalNotePlayerFactory : IPlayerFactory
	{
		public IPlayer Create()
		{
			return new SignalNotePlayer();
		}
	}
}
