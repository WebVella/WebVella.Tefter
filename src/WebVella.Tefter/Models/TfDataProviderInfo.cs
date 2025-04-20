namespace WebVella.Tefter.Models;

public class TfDataProviderInfo
{
	public Guid Id { get; internal set; }
	public string Name { get; internal set; }
	public int Index { get; internal set; }
	public long RowsCount { get; internal set; }
	public ITfDataProviderAddon ProviderType { get; internal set; }
}