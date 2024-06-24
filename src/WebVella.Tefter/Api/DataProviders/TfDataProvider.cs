namespace WebVella.Tefter;

internal class TfDataProvider
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public int Index { get; set; }
	public string CompositeKeyPrefix { get; set; }
	public ReadOnlyCollection<TfDataProviderColumn> Columns { get; set; }
	public ITfDataProviderType ProviderType { get; set; }
	
}
