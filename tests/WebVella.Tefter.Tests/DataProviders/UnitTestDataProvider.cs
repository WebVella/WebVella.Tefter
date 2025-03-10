using Bogus;
using System.Collections.ObjectModel;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Tests.DataProviders;

public class UnitTestDataProvider : ITfDataProviderType
{
	public Guid Id => new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d");

	public string Name => "UnitTest Data Provider";

	public string Description => "Used for unit test only";

	public string FluentIconName => "DocumentTable";

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

	public ReadOnlyCollection<TfDatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(
		string dataType)
	{
		switch (dataType)
		{
			case "TEXT":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Text }.AsReadOnly();
			case "SHORT_TEXT":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.ShortText }.AsReadOnly();
			case "BOOLEAN":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Boolean }.AsReadOnly();
			case "NUMBER":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Number }.AsReadOnly();
			case "DATE":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Date }.AsReadOnly();
			case "DATETIME":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateTime }.AsReadOnly();
			case "SHORT_INTEGER":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.ShortInteger }.AsReadOnly();
			case "INTEGER":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Integer }.AsReadOnly();
			case "LONG_INTEGER":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.LongInteger }.AsReadOnly();
			case "GUID":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Guid }.AsReadOnly();

		}
		return new List<TfDatabaseColumnType>().AsReadOnly();
	}

	public ReadOnlyCollection<TfDataProviderDataRow> GetRows(
		TfDataProvider provider)
	{
		List<Tuple<string, TfDatabaseColumnType, string>> columns = new List<Tuple<string, TfDatabaseColumnType, string>>();
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("guid_column", TfDatabaseColumnType.Guid, "GUID"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("short_text_column", TfDatabaseColumnType.ShortText, "SHORT_TEXT"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("text_column", TfDatabaseColumnType.Text, "TEXT"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("date_column", TfDatabaseColumnType.Date, "DATE"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("datetime_column", TfDatabaseColumnType.DateTime, "DATETIME"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("short_int_column", TfDatabaseColumnType.ShortInteger, "SHORT_INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("int_column", TfDatabaseColumnType.Integer, "INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("long_int_column", TfDatabaseColumnType.LongInteger, "LONG_INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("number_column", TfDatabaseColumnType.Number, "NUMBER"));

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

	private object GetRandomValueForDbColumnType(TfDatabaseColumnType dbType)
	{
		var faker = new Faker("en");
		switch (dbType)
		{
			case TfDatabaseColumnType.Guid:
				return faker.Random.Guid();
			case TfDatabaseColumnType.ShortText:
				return faker.Lorem.Sentence();
			case TfDatabaseColumnType.Text:
				return faker.Lorem.Lines();
			case TfDatabaseColumnType.ShortInteger:
				return faker.Random.Short(0, 100);
			case TfDatabaseColumnType.Integer:
				return faker.Random.Number(100, 1000);
			case TfDatabaseColumnType.LongInteger:
				return faker.Random.Long(1000, 10000);
			case TfDatabaseColumnType.Number:
				return faker.Random.Decimal(100000, 1000000);
			case TfDatabaseColumnType.Date:
				return faker.Date.PastDateOnly();
			case TfDatabaseColumnType.DateTime:
				return faker.Date.Future();
			default:
				throw new Exception("Type is not supported");
		}
	}

	public TfDataProviderSourceSchemaInfo GetDataProviderSourceSchema(TfDataProvider provider)
	{
		throw new NotImplementedException();
	}

	public List<ValidationError> Validate(string settingsJson)
	{
		throw new NotImplementedException();
	}
}