namespace WebVella.Tefter;

public interface ITfDataProviderType
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public string FluentIconName { get; }
	public abstract ReadOnlyCollection<string> GetSupportedSourceDataTypes();
	public abstract ReadOnlyCollection<TfDatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(
		string dataType);
	public abstract ReadOnlyCollection<TfDataProviderDataRow> GetRows(
		TfDataProvider provider);
	public abstract TfDataProviderSourceSchemaInfo GetDataProviderSourceSchema(TfDataProvider provider);

	public List<ValidationError> Validate(string settingsJson);
}
