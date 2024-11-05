namespace WebVella.Tefter.Database;

public record TfDatabaseUpdateResult
{
	private List<TfDatabaseUpdateLogRecord> _logs;
	public bool IsSuccess { get { return _logs.All(x => x.Success); } }
	public ReadOnlyCollection<TfDatabaseUpdateLogRecord> Log => _logs.AsReadOnly();

	internal TfDatabaseUpdateResult(List<TfDatabaseUpdateLogRecord> logs)
	{
		if (logs == null)
			throw new ArgumentNullException(nameof(logs));

		_logs = logs;
	}
}
