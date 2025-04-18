﻿using Bogus;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.Json;
using WebVella.Tefter.Database;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;
using WebVella.Tefter.Seeds.SampleDataProvider.Models;

namespace WebVella.Tefter.Seeds.SampleDataProvider;

public class SampleDataProvider : ITfDataProviderAddon
{
	public const string ID = "6c1efbe4-249a-472b-bfc6-7599310069e1";
	public const string NAME = "Sample Data Provider";
	public const string DESCRIPTION = "Provide hardcoded sample data";
	public const string FLUENT_ICON_NAME = "DocumentTable";

	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;

	/// <summary>
	/// Return what types of data types it can process from the data source
	/// </summary>
	public ReadOnlyCollection<string> GetSupportedSourceDataTypes()
	{
		//sample only
		return new List<string> {
			"TEXT", //Keep text on first place as the first place is used as default type
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
	/// <summary>
	/// Returns mapping between source data types and Tefter data types
	/// </summary>
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
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateOnly, TfDatabaseColumnType.DateTime }.AsReadOnly();
			case "DATETIME":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateTime, TfDatabaseColumnType.DateOnly }.AsReadOnly();
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

	/// <summary>
	/// Gets data from the data source. In this case we randomly generate data.
	/// </summary>
	/// <param name="provider"></param>
	/// <returns></returns>
	public ReadOnlyCollection<TfDataProviderDataRow> GetRows(
		TfDataProvider provider,
		ITfDataProviderSychronizationLog synchLog)
	{
		var result = new List<TfDataProviderDataRow>();
		var settings = JsonSerializer.Deserialize<SampleDataProviderSettings>(provider.SettingsJson);

		List<Tuple<string, TfDatabaseColumnType, string>> columns = new List<Tuple<string, TfDatabaseColumnType, string>>();
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("sample_guid_column", TfDatabaseColumnType.Guid, "GUID"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("sample_short_text_column", TfDatabaseColumnType.ShortText, "SHORT_TEXT"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("sample_text_column", TfDatabaseColumnType.Text, "TEXT"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("sample_date_column", TfDatabaseColumnType.DateOnly, "DATE"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("sample_datetime_column", TfDatabaseColumnType.DateTime, "DATETIME"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("sample_short_int_column", TfDatabaseColumnType.ShortInteger, "SHORT_INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("sample_int_column", TfDatabaseColumnType.Integer, "INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("sample_long_int_column", TfDatabaseColumnType.LongInteger, "LONG_INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("sample_number_column", TfDatabaseColumnType.Number, "NUMBER"));

		var rows = new List<TfDataProviderDataRow>();
		for (int i = 0; i < 100; i++)
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

	/// <summary>
	/// Generates random value for the given database column type using Bogus library
	/// </summary>
	/// <param name="dbType"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
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
			case TfDatabaseColumnType.DateOnly:
				return faker.Date.PastDateOnly();
			case TfDatabaseColumnType.DateTime:
				return faker.Date.Future();
			default:
				throw new Exception("Type is not supported");
		}
	}

	/// <summary>
	/// Gets the data source schema
	/// </summary>
	public TfDataProviderSourceSchemaInfo GetDataProviderSourceSchema(TfDataProvider provider)
	{
		var settings = JsonSerializer.Deserialize<SampleDataProviderSettings>(provider.SettingsJson);
		var result = new TfDataProviderSourceSchemaInfo();
		return result;
	}
	/// <summary>
	/// Validates the settings of the data provider
	/// </summary>
	/// <param name="settingsJson"></param>
	/// <returns></returns>
	public List<ValidationError> Validate(string settingsJson)
	{

		SampleDataProviderSettings settings = new();
		if (!String.IsNullOrWhiteSpace(settingsJson))
		{
			try
			{
				settings = JsonSerializer.Deserialize<SampleDataProviderSettings>(settingsJson);
			}
			catch { }
		}
		var errors = new List<ValidationError>();

		//simple validation for empty value
		if (String.IsNullOrWhiteSpace(settings.SampleSetting))
		{
			errors.Add(new ValidationError(nameof(SampleDataProviderSettings.SampleSetting), "Sample Setting requires value."));
		}

		return errors;
	}

	TfDataProviderSourceSchemaInfo ITfDataProviderAddon.GetDataProviderSourceSchema(
		TfDataProvider provider)
	{
		TfDataProviderSourceSchemaInfo schemaInfo = new TfDataProviderSourceSchemaInfo();
		SampleDataProviderSynchronizationLog synchLog = new();
		var rows = GetRows(provider, synchLog);
		if (rows.Count == 0)
			return schemaInfo;

		var columnNames = rows[0].ColumnNames;

		foreach (var columnName in columnNames)
		{
			object value = null;

			//try to find value in any row,
			//on first found exist cycle
			foreach (var row in rows)
			{
				if (row[columnName] != null)
				{
					value = row[columnName];
					break;
				}
			}

			if (value == null)
				continue;

			if (value.GetType() == typeof(Guid))
			{
				schemaInfo.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.Guid;
				schemaInfo.SourceColumnDefaultSourceType[columnName] = "GUID";
			}
			else if (value.GetType() == typeof(DateOnly))
			{
				schemaInfo.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.DateOnly;
				schemaInfo.SourceColumnDefaultSourceType[columnName] = "DATE";
			}
			else if (value.GetType() == typeof(DateTime))
			{
				schemaInfo.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.DateTime;
				schemaInfo.SourceColumnDefaultSourceType[columnName] = "DATETIME";
			}
			else if (value.GetType() == typeof(short))
			{
				schemaInfo.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.ShortInteger;
				schemaInfo.SourceColumnDefaultSourceType[columnName] = "SHORT_INTEGER";
			}
			else if (value.GetType() == typeof(int))
			{
				schemaInfo.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.Integer;
				schemaInfo.SourceColumnDefaultSourceType[columnName] = "INTEGER";
			}
			else if (value.GetType() == typeof(long))
			{
				schemaInfo.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.LongInteger;
				schemaInfo.SourceColumnDefaultSourceType[columnName] = "LONG_INTEGER";
			}
			else if (value.GetType() == typeof(decimal))
			{
				schemaInfo.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.Number;
				schemaInfo.SourceColumnDefaultSourceType[columnName] = "NUMBER";
			}
			else if (value.GetType() == typeof(string))
			{
				schemaInfo.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.Text;
				schemaInfo.SourceColumnDefaultSourceType[columnName] = "TEXT";
			}
			else if (value.GetType() == typeof(bool))
			{
				schemaInfo.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.Boolean;
				schemaInfo.SourceColumnDefaultSourceType[columnName] = "BOOLEAN";
			}

		}
		schemaInfo.SourceTypeSupportedDbTypes = new Dictionary<string, List<TfDatabaseColumnType>>();
		foreach (var providerDataType in provider.ProviderType.GetSupportedSourceDataTypes())
		{
			var supportedDBList = provider.ProviderType.GetDatabaseColumnTypesForSourceDataType(providerDataType);
			var supportedDbType = supportedDBList.Count > 0 ? supportedDBList.First() : TfDatabaseColumnType.Text;
			schemaInfo.SourceTypeSupportedDbTypes[providerDataType] = supportedDBList.ToList();
		}

		return schemaInfo;
	}
}
