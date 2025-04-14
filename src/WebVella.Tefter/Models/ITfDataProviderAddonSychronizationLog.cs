using Microsoft.FluentUI.AspNetCore.Components;

namespace WebVella.Tefter.Models;

public interface ITfDataProviderSychronizationLog
{
	public void Log(string message,
		TfDataProviderSychronizationLogEntryType type =
		TfDataProviderSychronizationLogEntryType.Info);

	public void Log(
		string message,
		Exception ex);

	internal ReadOnlyCollection<TfDataProviderSychronizationLogEntry> GetEntries();
}

internal class TfDataProviderSychronizationLog : ITfDataProviderSychronizationLog
{
	private readonly List<TfDataProviderSychronizationLogEntry> _entries;

	public TfDataProviderSychronizationLog()
	{
		_entries = new List<TfDataProviderSychronizationLogEntry>();
	}

	public TfDataProviderSychronizationLog(IEnumerable<TfDataProviderSychronizationLogEntry> entries)
	{
		if(entries == null)
			throw new ArgumentNullException(nameof(entries));

		_entries = new List<TfDataProviderSychronizationLogEntry>();
		_entries.AddRange(entries);
	}

	public ReadOnlyCollection<TfDataProviderSychronizationLogEntry> GetEntries()
	{
		return _entries.AsReadOnly();
	}

	public void Log(string message,
		TfDataProviderSychronizationLogEntryType type = 
		TfDataProviderSychronizationLogEntryType.Info )
	{
		_entries.Add(new TfDataProviderSychronizationLogEntry
		{
			CreatedOn = DateTime.Now,
			Message = message??string.Empty,
			Type =  type
		});
	}

	public void Log(string message, Exception ex)
	{
		_entries.Add(new TfDataProviderSychronizationLogEntry
		{
			CreatedOn = DateTime.Now,
			Message = message ?? string.Empty,
			Type =  TfDataProviderSychronizationLogEntryType.Error
		});

		if (ex == null)
			return;

		_entries.Add(new TfDataProviderSychronizationLogEntry
		{
			CreatedOn = DateTime.Now,
			Message = $"exception message:{ex.Message}",
			Type = TfDataProviderSychronizationLogEntryType.Error
		});

		_entries.Add(new TfDataProviderSychronizationLogEntry
		{
			CreatedOn = DateTime.Now,
			Message = $"exception stacktrace:{ex.StackTrace}",
			Type = TfDataProviderSychronizationLogEntryType.Error
		});
	}
}

public record TfDataProviderSychronizationLogEntry
{
	public DateTime CreatedOn { get; set; }
	public string Message { get; set; }
	public TfDataProviderSychronizationLogEntryType Type { get; set; } =
		TfDataProviderSychronizationLogEntryType.Info;
}

public enum TfDataProviderSychronizationLogEntryType
{
	Info,
	Error
}