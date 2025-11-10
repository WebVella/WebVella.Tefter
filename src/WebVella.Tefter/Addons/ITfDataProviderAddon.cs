namespace WebVella.Tefter.Addons;

public interface ITfDataProviderAddon : ITfAddon
{
	public abstract ReadOnlyCollection<string> GetSupportedSourceDataTypes();

	public abstract ReadOnlyCollection<TfDatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(
		string dataType);

	public abstract ReadOnlyCollection<TfDataProviderDataRow> GetRows(
		TfDataProvider provider,
		ITfDataProviderSychronizationLog log);

	public abstract TfDataProviderSourceSchemaInfo GetDataProviderSourceSchema(
		TfDataProvider provider);

	public List<ValidationError> Validate(
		string settingsJson);
	
	public abstract Task CanBeCreatedFromFile(
		TfImportFileToPageContextItem item);	

	public abstract Task CreateFromFile(
		TfImportFileToPageContextItem item);
}
