using System.Collections.ObjectModel;

namespace WebVella.Tefter.Tests.DataProviders;

public class UnitTestDataProvider : ITfDataProviderType
{
	public Guid Id => new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d");

	public string Name => "UnitTest Data Provider";

	public string Description => "Used for unit test only";

	public string IconBase64 => "";
	public Stream Icon => GetType().Assembly.GetManifestResourceStream("WebVella.Tefter.Tests.DataProviders.Csv.Assets.Icon.png");

	public Type SettingsComponentType => typeof(UnitTestDataProviderSettingsComponent);

	public ReadOnlyCollection<DatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(string dataType)
	{
		switch(dataType)
		{
			case "TEXT":
				return new List<DatabaseColumnType> { DatabaseColumnType.Text}.AsReadOnly();
			case "SHORT_TEXT":
				return new List<DatabaseColumnType> { DatabaseColumnType.ShortText }.AsReadOnly();
			case "BOOLEAN":
				return new List<DatabaseColumnType> { DatabaseColumnType.Boolean }.AsReadOnly();
			case "NUMBER":
				return new List<DatabaseColumnType> { DatabaseColumnType.Number }.AsReadOnly();
			case "DATE":
				return new List<DatabaseColumnType> { DatabaseColumnType.Date }.AsReadOnly();
			case "DATETIME":
				return new List<DatabaseColumnType> { DatabaseColumnType.DateTime }.AsReadOnly();
			case "SHORT_INTEGER":
				return new List<DatabaseColumnType> { DatabaseColumnType.ShortInteger }.AsReadOnly();
			case "INTEGER":
				return new List<DatabaseColumnType> { DatabaseColumnType.Integer }.AsReadOnly();
			case "LONG_INTEGER":
				return new List<DatabaseColumnType> { DatabaseColumnType.LongInteger }.AsReadOnly();
			case "GUID":
				return new List<DatabaseColumnType> { DatabaseColumnType.Guid }.AsReadOnly();

		}
		return new List<DatabaseColumnType>().AsReadOnly();
	}

	public ReadOnlyCollection<TfDataProviderDataRow> GetSourceData()
	{
		return new List<TfDataProviderDataRow>().AsReadOnly();
	}

	public ReadOnlyCollection<string> GetSupportedSourceDataTypes()
	{
		return new List<string> { 
			"TEXT", 
			"SHORT_TEXT",
			"BOOLEAN",
			"DATE", 
			"DATETIME",
			"NUMBER",
			"SHORT_INTEGER",
			"INTEGER",
			"LONG_INTEGER",
			"GUID"
		}.AsReadOnly();
	}
}
