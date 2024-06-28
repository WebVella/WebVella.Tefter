namespace WebVella.Tefter.DataProviders.Csv;

public class CsvDataProvider : ITfDataProviderType
{
	public Guid Id => new Guid("82883b60-197f-4f5a-8c6a-2bec16508816");

	public string Name => "Csv Data Provider";

	public string Description => "Provide data from CSV formated file.";

	public Stream Icon => GetType().Assembly.GetManifestResourceStream(Constants.CSV_DATA_PROVIDER_ICON);

	public Type SettingsComponentType => typeof(DataProviderSettingsComponent);

	public ReadOnlyCollection<DatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(string dataType)
	{
		switch(dataType)
		{
			case "TEXT":
				return new List<DatabaseColumnType> { DatabaseColumnType.Text}.AsReadOnly();
			case "NUMBER":
				return new List<DatabaseColumnType> { DatabaseColumnType.Number }.AsReadOnly();
			case "DATE":
				return new List<DatabaseColumnType> { DatabaseColumnType.Date, DatabaseColumnType.DateTime }.AsReadOnly();
			case "DATETIME":
				return new List<DatabaseColumnType> { DatabaseColumnType.DateTime }.AsReadOnly();

		}
		return new List<DatabaseColumnType>().AsReadOnly();
	}

	public ReadOnlyCollection<TfDataProviderDataRow> GetSourceData()
	{
		return new List<TfDataProviderDataRow>().AsReadOnly();
	}

	public ReadOnlyCollection<string> GetSupportedSourceDataTypes()
	{
		//sample only
		return new List<string> { "TEXT", "DATE", "DATETIME", "NUMBER" }.AsReadOnly();
	}
}
