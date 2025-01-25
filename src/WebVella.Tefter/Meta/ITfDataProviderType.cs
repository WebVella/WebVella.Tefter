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

public class TfDataProviderTypeMeta
{
	public Guid Id { get { return Instance.Id; } }
	public string Name { get { return Instance.Name; } }
	public string Description { get { return Instance.Description; } }
	public string FluentIconName { get { return Instance.FluentIconName; } }
	public ITfDataProviderType Instance { get; init; }
}

