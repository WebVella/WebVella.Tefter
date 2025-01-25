using CsvHelper;
using CsvHelper.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.DataProviders.Csv;

public class CsvDataProvider : ITfDataProviderType
{
	public Guid Id => new Guid("82883b60-197f-4f5a-8c6a-2bec16508816");

	public string Name => "Csv Data Provider";

	public string Description => "Provide data from CSV formated file.";

	public string FluentIconName => "DocumentTable";

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
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Date, TfDatabaseColumnType.DateTime }.AsReadOnly();
			case "DATETIME":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateTime, TfDatabaseColumnType.Date }.AsReadOnly();
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

			if (string.IsNullOrWhiteSpace(settings.Filepath))
				throw new Exception("Provider csv file path is not specified.");

			if (settings.Filepath.ToLowerInvariant().StartsWith("tefter://"))
			{
				var repoService = provider.ServiceProvider.GetService<ITfRepositoryService>();

				var file = repoService.GetFileByUri(settings.Filepath).Value;

				if (file is null)
					throw new Exception($"File '{settings.Filepath}' is not found.");

				using (var stream = repoService.GetFileContentAsFileStream(file.Filename).Value)
				{
					return ReadCSVStream(stream, provider, config, settings, culture);
				}
			}
			else
			{
				using (var stream = new FileStream(settings.Filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					return ReadCSVStream(stream, provider, config, settings, culture);
				}
			}

		}
		finally
		{
			Thread.CurrentThread.CurrentCulture = currentCulture;
			Thread.CurrentThread.CurrentUICulture = currentUICulture;
		}
	}

	public TfDataProviderSourceSchemaInfo GetDataProviderSourceSchema(TfDataProvider provider)
	{
		int maxSampleSize = 25;
		var settings = JsonSerializer.Deserialize<CsvDataProviderSettings>(provider.SettingsJson);
		var culture = new CultureInfo(settings.CultureName);
		var result = new TfDataProviderSourceSchemaInfo();
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
		Stream stream;
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
		if (string.IsNullOrWhiteSpace(settings.Filepath))
			throw new Exception("Provider csv file path is not specified.");

		if (settings.Filepath.ToLowerInvariant().StartsWith("tefter://"))
		{
			var repoService = provider.ServiceProvider.GetService<ITfRepositoryService>();

			var file = repoService.GetFileByUri(settings.Filepath).Value;

			if (file is null)
				throw new Exception($"File '{settings.Filepath}' is not found.");

			stream = repoService.GetFileContentAsFileStream(file.Filename).Value;
		}
		else
		{
			stream = new FileStream(settings.Filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}
		Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes = new();
		using (var reader = new StreamReader(stream))
		using (var csvReader = new CsvReader(reader, config))
		{
			csvReader.Read();
			csvReader.ReadHeader();
			foreach (var item in csvReader.HeaderRecord)
			{
				result.SourceColumnDefaultDbType[item] = TfDatabaseColumnType.Text;
			}
			var counter = 0;
			while (csvReader.Read())
			{
				if (counter >= maxSampleSize) break;
				foreach (var column in csvReader.HeaderRecord)
				{
					var fieldValue = csvReader.GetField(column);
					if (!suggestedColumnTypes.ContainsKey(column)) suggestedColumnTypes[column] = new();
					TfDatabaseColumnType type = CsvSourceToColumnTypeConverter.GetDataTypeFromString(fieldValue, culture);
					suggestedColumnTypes[column].Add(type);
				}
				counter++;
			}
		}
		foreach (var key in result.SourceColumnDefaultDbType.Keys)
		{
			var columnType = TfDatabaseColumnType.Text;
			if (suggestedColumnTypes.ContainsKey(key))
				columnType = CsvSourceToColumnTypeConverter.GetTypeFromOptions(suggestedColumnTypes[key]);

			result.SourceColumnDefaultDbType[key] = columnType;
		}
		var preferredSourceTypeForDbType = new Dictionary<TfDatabaseColumnType, string>();
		foreach (var providerDataType in provider.ProviderType.GetSupportedSourceDataTypes())
		{
			var supportedDBList = provider.ProviderType.GetDatabaseColumnTypesForSourceDataType(providerDataType);
			var supportedDbType = supportedDBList.Count > 0 ? supportedDBList.First() : TfDatabaseColumnType.Text;
			result.SourceTypeSupportedDbTypes[providerDataType] = supportedDBList.ToList();
			if (supportedDBList.Count > 0)
			{
				preferredSourceTypeForDbType[supportedDBList.First()] = providerDataType;
			}
		}
		foreach (var columnName in result.SourceColumnDefaultDbType.Keys)
		{
			var dbType = result.SourceColumnDefaultDbType[columnName];
			if (preferredSourceTypeForDbType.ContainsKey(dbType))
				result.SourceColumnDefaultSourceType[columnName] = preferredSourceTypeForDbType[dbType];
			else
				result.SourceColumnDefaultSourceType[columnName] = GetSupportedSourceDataTypes().First();
		}

		return result;
	}

	private ReadOnlyCollection<TfDataProviderDataRow> ReadCSVStream(
		Stream stream,
		TfDataProvider provider,
		CsvConfiguration config,
		CsvDataProviderSettings settings,
		CultureInfo culture)
	{
		var result = new List<TfDataProviderDataRow>();

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
							throw new Exception($"Source column '{sourceName}' is not found in csv.");
						}

						row[providerColumnWithSource.DbName] = ConvertValue(
							providerColumnWithSource,
							sourceRow[sourceName],
							settings: settings,
							culture: culture);

					}
					catch (Exception ex)
					{
						var value = sourceRow[sourceName];
						throw new Exception($"Exception while processing source row: {ex.Message}");
					}
				}
				result.Add(row);
			}
		}

		return result.AsReadOnly();
	}

	private object ConvertValue(
		TfDataProviderColumn column,
		object value,
		CsvDataProviderSettings settings,
		CultureInfo culture)
	{
		//CSV source values are all string
		string stringValue = value?.ToString();

		//if column is nullable return null, null is return for empty string 
		if ((string.IsNullOrEmpty(stringValue) || stringValue?.ToLowerInvariant() == "null")
			&& column.IsNullable)
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
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				return stringValue;

			case TfDatabaseColumnType.Boolean:
				{
					if (Boolean.TryParse(value?.ToString(), out bool parsedValue))
						return parsedValue;

					throw new Exception($"Cannot convert value to boolean value for column {column.SourceName}");
				}

			case TfDatabaseColumnType.Guid:
				{
					if (Guid.TryParse(value?.ToString(), out Guid parsedValue))
						return parsedValue;

					throw new Exception("Cannot convert value to GUID value for column {column.SourceName}");
				}

			case TfDatabaseColumnType.DateTime:
				{
					if (!String.IsNullOrWhiteSpace(columnImportParseFormat)
						&& DateTime.TryParseExact(value?.ToString(), columnImportParseFormat, culture, DateTimeStyles.AssumeLocal, out DateTime parsedValueExact))
						return parsedValueExact;
					else if (DateTime.TryParse(value?.ToString(), out DateTime parsedValue))
						return parsedValue;

					throw new Exception($"Cannot convert value to DateTime value for column {column.SourceName}");
				}

			case TfDatabaseColumnType.Date:
				{
					if (!String.IsNullOrWhiteSpace(columnImportParseFormat)
						&& DateTime.TryParseExact(value?.ToString(), columnImportParseFormat, culture, DateTimeStyles.AssumeLocal, out DateTime parsedValueExact))
					{
						//There are problems with DateOnly parse exact, so we use DateTime
						return new DateOnly(parsedValueExact.Year, parsedValueExact.Month, parsedValueExact.Day);
					}
					else if (DateOnly.TryParse(value?.ToString(), out DateOnly parsedValue))
						return parsedValue;

					throw new Exception($"Cannot convert value to DateOnly value for column {column.SourceName}");
				}

			case TfDatabaseColumnType.ShortInteger:
				{
					if (short.TryParse(value?.ToString(), out short parsedValue))
						return parsedValue;

					throw new Exception($"Cannot convert value to ShortInteger value for column {column.SourceName}");
				}

			case TfDatabaseColumnType.Integer:
				{
					if (int.TryParse(value?.ToString(), out int parsedValue))
						return parsedValue;

					throw new Exception($"Cannot convert value to Integer value for column {column.SourceName}");
				}

			case TfDatabaseColumnType.LongInteger:
				{
					if (long.TryParse(value?.ToString(), out long parsedValue))
						return parsedValue;

					throw new Exception($"Cannot convert value to LongInteger value for column {column.SourceName}");
				}

			case TfDatabaseColumnType.Number:
				{
					if (decimal.TryParse(value?.ToString(), out decimal parsedValue))
						return parsedValue;

					throw new Exception($"Cannot convert value to Number value for column {column.SourceName}");
				}

			default:
				throw new Exception($"Not supported source type for column {column.SourceName}");
		}
	}

	public List<ValidationError> Validate(string settingsJson)
	{

		CsvDataProviderSettings settings = new();
		if (!String.IsNullOrWhiteSpace(settingsJson))
		{
			try
			{
				settings = JsonSerializer.Deserialize<CsvDataProviderSettings>(settingsJson);
			}
			catch { }
		}
		var errors = new List<ValidationError>();

		if (String.IsNullOrWhiteSpace(settings.Filepath))
		{
			errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("required")));
		}
		else
		{
			string extension = Path.GetExtension(settings.Filepath);
			if (extension != ".csv")
			{
				errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("'csv' file extension is required")));
			}
		}

		if (!String.IsNullOrWhiteSpace(settings.CultureName))
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
			var culture = cultures.FirstOrDefault(c => c.Name.Equals(settings.CultureName, StringComparison.OrdinalIgnoreCase));
			if (culture == null)
				errors.Add(new ValidationError(nameof(CsvDataProviderSettings.CultureName), LOC("invalid. format like 'en-US'")));
		}
		return errors;
	}

	private string LOC(string name) => name;
}
