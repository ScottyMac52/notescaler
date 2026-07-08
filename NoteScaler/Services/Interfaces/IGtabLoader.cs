namespace NoteScaler.Services.Interfaces
{
	using NoteScaler.Models;

	public interface IGtabLoader
	{
		bool Load(string gtabName, out string errorString, out Tablature tablature);
	}
}
