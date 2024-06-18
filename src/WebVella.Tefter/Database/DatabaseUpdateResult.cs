namespace WebVella.Tefter.Database;

public record DatabaseUpdateResult
{
	private List<DatabaseUpdateLogRecord> _logs;
	public bool IsSuccess { get { return _logs.All(x => x.Success); } }
	public ReadOnlyCollection<DatabaseUpdateLogRecord> Log => _logs.AsReadOnly();

	internal DatabaseUpdateResult(List<DatabaseUpdateLogRecord> logs)
	{
		if (logs == null)
			throw new ArgumentNullException(nameof(logs));

		_logs = logs;
	}
}
