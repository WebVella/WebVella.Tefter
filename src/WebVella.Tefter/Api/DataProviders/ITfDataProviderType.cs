namespace WebVella.Tefter;

public interface ITfDataProviderType
{
	Guid Id { get; }
	string Name { get; }
	string Description { get; }
	Stream Icon { get; }
	Type OptionsComponentType { get; }
	public abstract void LoadSettings(string settingJson);
	public abstract ReadOnlyCollection<string> GetSupportedSourceDataTypes();
	public abstract ReadOnlyCollection<DatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(string dataType);
	public abstract ReadOnlyCollection<TfDataProviderDataRow> GetSourceData();
}
