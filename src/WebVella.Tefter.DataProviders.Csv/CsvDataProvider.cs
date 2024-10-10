﻿using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace WebVella.Tefter.DataProviders.Csv;

public class CsvDataProvider : ITfDataProviderType
{
	public Guid Id => new Guid("82883b60-197f-4f5a-8c6a-2bec16508816");

	public string Name => "Csv Data Provider";

	public string Description => "Provide data from CSV formated file.";

	public string ImageBase64
	{
		get
		{
			var stream = GetType().Assembly.GetManifestResourceStream(Constants.CSV_DATA_PROVIDER_ICON);
			byte[] bytes;
			using (var memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				bytes = memoryStream.ToArray();
			}
			return Convert.ToBase64String(bytes);
		}
	}

	public Type SettingsComponentType => typeof(DataProviderSettingsComponent);

	public ReadOnlyCollection<string> GetSupportedSourceDataTypes()
	{
		//sample only
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

		var currentCulture = Thread.CurrentThread.CurrentCulture;
		var currentUICulture = Thread.CurrentThread.CurrentUICulture;

		var result = new List<TfDataProviderDataRow>();

		try
		{
			var settings = JsonSerializer.Deserialize<CsvDataProviderSettings>(provider.SettingsJson);
			var culture = new CultureInfo(settings.CultureName);

			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentCulture = culture;

			var config = new CsvConfiguration(culture)
			{
				PrepareHeaderForMatch = args => args.Header.ToLower(),
				Encoding = Encoding.UTF8,
				IgnoreBlankLines = true,
				BadDataFound = null,
				TrimOptions = TrimOptions.Trim,
				HasHeaderRecord = true,
				MissingFieldFound = null
			};

			switch (settings.Delimter)
			{
				case CsvDataProviderSettingsDelimiter.Semicolon:
					config.Delimiter = ";";
					break;
				case CsvDataProviderSettingsDelimiter.Tab:
					config.Delimiter = "\t";
					break;
				default:
					config.Delimiter = ",";
					break;
			}

			using (var stream = new FileStream(settings.Filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var reader = new StreamReader(stream))
			using (var csvReader = new CsvReader(reader, config))
			{

				csvReader.Read();
				csvReader.ReadHeader();

				var sourceColumns = provider.Columns.Where(x => !string.IsNullOrWhiteSpace(x.SourceName));

				while (csvReader.Read())
				{

					Dictionary<string, string> sourceRow = new Dictionary<string, string>();
					foreach (var column in csvReader.HeaderRecord)
						sourceRow[column] = csvReader.GetField(column);


					TfDataProviderDataRow row = new TfDataProviderDataRow();
					foreach (var providerColumnWithSource in sourceColumns)
					{
						var sourceName = providerColumnWithSource.SourceName.Trim();
						try
						{
							if (!sourceRow.ContainsKey(sourceName))
							{
								row.AddError($"Source column '{sourceName}' is not found in csv.");
								continue;
							}

							row[providerColumnWithSource.DbName] = ConvertValue(
								providerColumnWithSource,
								sourceRow[sourceName],
								settings: settings);

						}
						catch (Exception ex)
						{
							var value = sourceRow[sourceName];
							row.AddError($"Exception while processing source row: {ex.Message}");
						}
					}
					result.Add(row);
				}
			}
		}
		finally
		{
			Thread.CurrentThread.CurrentCulture = currentCulture;
			Thread.CurrentThread.CurrentUICulture = currentUICulture;
		}

		return result.AsReadOnly();
	}

	private object ConvertValue(
		TfDataProviderColumn column,
		object value,
		CsvDataProviderSettings settings)
	{
		//CSV source values are all string
		string stringValue = value?.ToString();

		//if column is nullable return null, null is return for empty string 
		if (string.IsNullOrEmpty(stringValue) && column.IsNullable)
			return null;

		string columnImportParseFormat = null;
		if (settings is not null && settings.AdvancedSetting is not null
			&& settings.AdvancedSetting.ColumnImportParseFormat is not null
			&& settings.AdvancedSetting.ColumnImportParseFormat.ContainsKey(column.DbName))
		{
			columnImportParseFormat = settings.AdvancedSetting.ColumnImportParseFormat[column.DbName];
		}

		switch (column.DbType)
		{
			case DatabaseColumnType.ShortText:
			case DatabaseColumnType.Text:
				return stringValue;

			case DatabaseColumnType.Boolean:
				{
					if (Boolean.TryParse(value?.ToString(), out bool parsedValue))
						return parsedValue;

					throw new Exception("Cannot convert value to boolean value");
				}

			case DatabaseColumnType.Guid:
				{
					if (Guid.TryParse(value?.ToString(), out Guid parsedValue))
						return parsedValue;

					throw new Exception("Cannot convert value to GUID value");
				}

			case DatabaseColumnType.DateTime:
				{
					if (!String.IsNullOrWhiteSpace(columnImportParseFormat)
						&& DateTime.TryParseExact(value?.ToString(), columnImportParseFormat, null, DateTimeStyles.AssumeLocal, out DateTime parsedValueExact))
						return parsedValueExact;
					else if (DateTime.TryParse(value?.ToString(), out DateTime parsedValue))
						return parsedValue;

					throw new Exception("Cannot convert value to DateTime value");
				}

			case DatabaseColumnType.Date:
				{
					if (!String.IsNullOrWhiteSpace(columnImportParseFormat)
						&& DateTime.TryParseExact(value?.ToString(), columnImportParseFormat, null, DateTimeStyles.AssumeLocal, out DateTime parsedValueExact))
					{
						//There are problems with DateOnly parse exact, so we use DateTime
						return new DateOnly(parsedValueExact.Year,parsedValueExact.Month,parsedValueExact.Day);
					}
					else if (DateOnly.TryParse(value?.ToString(), out DateOnly parsedValue))
						return parsedValue;

					throw new Exception("Cannot convert value to DateOnly value");
				}

			case DatabaseColumnType.ShortInteger:
				{
					if (short.TryParse(value?.ToString(), out short parsedValue))
						return parsedValue;

					throw new Exception("Cannot convert value to ShortInteger value");
				}

			case DatabaseColumnType.Integer:
				{
					if (int.TryParse(value?.ToString(), out int parsedValue))
						return parsedValue;

					throw new Exception("Cannot convert value to Integer value");
				}

			case DatabaseColumnType.LongInteger:
				{
					if (long.TryParse(value?.ToString(), out long parsedValue))
						return parsedValue;

					throw new Exception("Cannot convert value to LongInteger value");
				}

			case DatabaseColumnType.Number:
				{
					if (decimal.TryParse(value?.ToString(), out decimal parsedValue))
						return parsedValue;

					throw new Exception("Cannot convert value to Number value");
				}

			default:
				throw new Exception("Not supported source type");
		}
	}


}
