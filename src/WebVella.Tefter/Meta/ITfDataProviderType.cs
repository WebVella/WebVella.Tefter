namespace WebVella.Tefter;

public interface ITfDataProviderType
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public string ImageBase64 { get; }
	Type SettingsComponentType { get; }
	public abstract ReadOnlyCollection<string> GetSupportedSourceDataTypes();
	public abstract ReadOnlyCollection<DatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(
		string dataType);
	public abstract ReadOnlyCollection<TfDataProviderDataRow> GetRows(
		TfDataProvider provider);

}

public class TfDataProviderTypeMeta
{
	public Guid Id { get { return Instance.Id; } }
	public string Name { get { return Instance.Name; } }
	public string Description { get { return Instance.Description; } }
	public string ImageBase64 { get { return Instance.ImageBase64; } }
	public Type SettingsComponentType { get { return Instance.SettingsComponentType; } }
	public ITfDataProviderType Instance { get; init; }
}

