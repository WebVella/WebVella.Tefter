namespace WebVella.Tefter;

public interface ITfDataProviderType
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public string FluentIconName { get; }
	Type SettingsComponentType { get; }
	public abstract ReadOnlyCollection<string> GetSupportedSourceDataTypes();
	public abstract ReadOnlyCollection<TfDatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(
		string dataType);
	public abstract ReadOnlyCollection<TfDataProviderDataRow> GetRows(
		TfDataProvider provider);

	public abstract Dictionary<string, TfDatabaseColumnType> GetDataProviderSourceSchema(TfDataProvider provider);

}

public class TfDataProviderTypeMeta
{
	public Guid Id { get { return Instance.Id; } }
	public string Name { get { return Instance.Name; } }
	public string Description { get { return Instance.Description; } }
	public string FluentIconName { get { return Instance.FluentIconName; } }
	public Type SettingsComponentType { get { return Instance.SettingsComponentType; } }
	public ITfDataProviderType Instance { get; init; }
}

