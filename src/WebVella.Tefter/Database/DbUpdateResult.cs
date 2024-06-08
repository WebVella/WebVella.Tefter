namespace WebVella.Tefter.Database;

public record DbUpdateResult
{
	private List<DbUpdateLogRecord> _logs;
	public bool IsSuccess { get { return _logs.All(x => x.Success); } }
	public ReadOnlyCollection<DbUpdateLogRecord> Log => _logs.AsReadOnly();

	internal DbUpdateResult(List<DbUpdateLogRecord> logs)
	{
		if (logs == null)
			throw new ArgumentNullException(nameof(logs));

		_logs = logs;
	}
}
