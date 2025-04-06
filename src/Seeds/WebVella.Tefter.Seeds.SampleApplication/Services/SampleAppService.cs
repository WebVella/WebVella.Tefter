
namespace WebVella.Tefter.Seeds.SampleApplication.Services;

public partial interface ISampleAppService
{
	List<Note> GetNotes();
	Note AddNote(string noteText);
	void UpdateNote(Guid id, string noteText);
	void DeleteNote(Guid id);
}

internal partial class SampleAppService : ISampleAppService
{
	public readonly ITfDatabaseService _dbService;
	public readonly ITfService _tfService;

	public SampleAppService(
		ITfDatabaseService dbService,
		ITfService tfService)
	{
		_dbService = dbService;
		_tfService = tfService;
	}

	public List<Note> GetNotes()
	{
		var result = new List<Note>();

		var dataTable = _dbService.ExecuteSqlQueryCommand("SELECT * FROM sample_app_notes ORDER BY created_on DESC");
		if (dataTable == null || dataTable.Rows.Count == 0)
			return result;

		foreach (DataRow row in dataTable.Rows)
		{
			Note note = new Note();
			note.Id = row.Field<Guid>("id");
			note.NoteText = row.Field<string>("note_text");
			note.CreatedOn = row.Field<DateTime>("created_on");
			result.Add(note);
		}

		return result;
	}

	public Note AddNote(string noteText)
	{
		Note note = new Note
		{
			Id = Guid.NewGuid(),
			NoteText = noteText,
			CreatedOn = DateTime.UtcNow
		};

		int result = _dbService.ExecuteSqlNonQueryCommand("INSERT INTO sample_app_notes (id,note_text,created_on)" +
			" VALUES(@id,@note_text,@created_on)",
			new NpgsqlParameter("id", note.Id),
			new NpgsqlParameter("note_text", note.NoteText),
			new NpgsqlParameter("created_on", note.CreatedOn));

		if (result != 1)
			throw new Exception("Failed to insert note into database");

		return note;
	}

	public void UpdateNote(Guid id, string noteText)
	{
		int result = _dbService.ExecuteSqlNonQueryCommand("UPDATE sample_app_notes  " +
			"SET note_text = @note_text " +
			"WHERE id = @id",
			new NpgsqlParameter("id", id),
			new NpgsqlParameter("note_text", noteText));

		if (result != 1)
			throw new Exception("Failed to update note into database");
	}

	public void DeleteNote(Guid id)
	{
		int result = _dbService.ExecuteSqlNonQueryCommand(
			"DELETE FROM sample_app_notes WHERE id = @id",
			new NpgsqlParameter("id", id));

		if (result != 1)
			throw new Exception("Failed to update note into database");
	}
}
