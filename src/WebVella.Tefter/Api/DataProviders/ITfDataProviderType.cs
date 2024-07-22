namespace WebVella.Tefter;

public interface ITfDataProviderType
{
	Guid Id { get; }
	string Name { get; }
	string Description { get; }
	string IconBase64 { get; }
	Type SettingsComponentType { get; }
	abstract ReadOnlyCollection<string> GetSupportedSourceDataTypes();
	abstract ReadOnlyCollection<DatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(
		string dataType);
	abstract ReadOnlyCollection<TfDataProviderDataRow> GetRows(
		TfDataProvider provider);

}
