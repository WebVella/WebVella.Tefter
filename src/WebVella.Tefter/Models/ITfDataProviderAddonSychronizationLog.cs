namespace WebVella.Tefter.Models;

public interface ITfDataProviderSychronizationLog
{
	public void AddNewLogEntry(string message);
}

internal class TfDataProviderSychronizationLog : ITfDataProviderSychronizationLog
{
	private readonly List<TfDataProviderSychronizationLogEntry> _entries;

	public TfDataProviderSychronizationLog()
	{
		_entries = new List<TfDataProviderSychronizationLogEntry>();
	}

	public ReadOnlyCollection<TfDataProviderSychronizationLogEntry> GetEntries()
	{
		return _entries.AsReadOnly();
	}

	public void AddNewLogEntry(string message)
	{
		_entries.Add(new TfDataProviderSychronizationLogEntry
		{
			CreatedOn = DateTime.Now,
			Message = message
		});
	}
}

internal record TfDataProviderSychronizationLogEntry
{
	public DateTime CreatedOn { get; set; }
	public string Message { get; set; }
}
