namespace NoteScaler.Models
{
	using NoteScaler.Enums;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class TuningDefinition
	{
		private readonly IReadOnlyList<string> openStringNotes;

		public TuningDefinition(TuningScheme tuningScheme, IEnumerable<string> openStringNotes)
		{
			if (openStringNotes == null)
			{
				throw new ArgumentNullException(nameof(openStringNotes));
			}

			var notes = openStringNotes.ToArray();
			if (notes.Length == 0)
			{
				throw new ArgumentException("A tuning definition must include at least one open string note.", nameof(openStringNotes));
			}

			if (notes.Any(string.IsNullOrWhiteSpace))
			{
				throw new ArgumentException("A tuning definition cannot contain empty open string notes.", nameof(openStringNotes));
			}

			TuningScheme = tuningScheme;
			this.openStringNotes = Array.AsReadOnly(notes);
		}

		public TuningScheme TuningScheme { get; }

		public IReadOnlyList<string> OpenStringNotes => openStringNotes;
	}
}
