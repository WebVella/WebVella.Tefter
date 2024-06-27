namespace WebVella.Tefter;

public class TfDataProvider
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public int Index { get; set; }
	public string CompositeKeyPrefix { get; set; } = null;
	public string SettingsJson { get; set; }
	public ReadOnlyCollection<TfDataProviderColumn> Columns { get; set; }
	public ITfDataProviderType ProviderType { get; set; }
	public ReadOnlyCollection<string> SupportedSourceDataTypes => ProviderType.GetSupportedSourceDataTypes();

}
