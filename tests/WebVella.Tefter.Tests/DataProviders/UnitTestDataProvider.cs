using Bogus;
using System.Collections.ObjectModel;

namespace WebVella.Tefter.Tests.DataProviders;

public class UnitTestDataProvider : ITfDataProviderType
{
	public Guid Id => new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d");

	public string Name => "UnitTest Data Provider";

	public string Description => "Used for unit test only";

	public string ImageBase64 => "";
	public Stream Icon => GetType().Assembly.GetManifestResourceStream("WebVella.Tefter.Tests.DataProviders.Csv.Assets.Icon.png");

	public Type SettingsComponentType => typeof(UnitTestDataProviderSettingsComponent);

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

	public ReadOnlyCollection<DatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(
		string dataType)
	{
		switch (dataType)
		{
			case "TEXT":
				return new List<DatabaseColumnType> { DatabaseColumnType.Text }.AsReadOnly();
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

	public ReadOnlyCollection<TfDataProviderDataRow> GetRows(
		TfDataProvider provider)
	{
		List<Tuple<string, DatabaseColumnType, string>> columns = new List<Tuple<string, DatabaseColumnType, string>>();
		columns.Add(new Tuple<string, DatabaseColumnType, string>("guid_column", DatabaseColumnType.Guid, "GUID"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("short_test_column", DatabaseColumnType.ShortText, "SHORT_TEXT"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("test_column", DatabaseColumnType.Text, "TEXT"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("date_column", DatabaseColumnType.Date, "DATE"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("datetime_column", DatabaseColumnType.DateTime, "DATETIME"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("short_int_column", DatabaseColumnType.ShortInteger, "SHORT_INTEGER"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("int_column", DatabaseColumnType.Integer, "INTEGER"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("long_int_column", DatabaseColumnType.LongInteger, "LONG_INTEGER"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("number_column", DatabaseColumnType.Number, "NUMBER"));

		var rows = new List<TfDataProviderDataRow>();
		for ( int i = 0; i < 100; i++ )
		{
			TfDataProviderDataRow tfDataProviderDataRow = new TfDataProviderDataRow();
			foreach (var column in columns)
			{
				tfDataProviderDataRow[column.Item1] = GetRandomValueForDbColumnType(column.Item2);
			}
			rows.Add(tfDataProviderDataRow);
		}
		return rows.AsReadOnly();
	}

	private object GetRandomValueForDbColumnType(DatabaseColumnType dbType)
	{
		var faker = new Faker("en");
		switch (dbType)
		{
			case DatabaseColumnType.Guid:
				return faker.Random.Guid();
			case DatabaseColumnType.ShortText:
				return faker.Lorem.Sentence();
			case DatabaseColumnType.Text:
				return faker.Lorem.Lines();
			case DatabaseColumnType.ShortInteger:
				return faker.Random.Short(0, 100);
			case DatabaseColumnType.Integer:
				return faker.Random.Number(100, 1000);
			case DatabaseColumnType.LongInteger:
				return faker.Random.Long(1000, 10000);
			case DatabaseColumnType.Number:
				return faker.Random.Decimal(100000, 1000000);
			case DatabaseColumnType.Date:
				return faker.Date.PastDateOnly();
			case DatabaseColumnType.DateTime:
				return faker.Date.Future();
			default:
				throw new Exception("Type is not supported");
		}
	}
}