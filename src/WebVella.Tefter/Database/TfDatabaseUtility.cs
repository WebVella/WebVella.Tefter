namespace WebVella.Tefter.Database;

internal class TfDatabaseUtility
{
    public static bool IsValidDbObjectName(string name, out string error)
    {
        error = null;

        if (string.IsNullOrEmpty(name))
        {
            error = "Name is required and cannot be empty";
            return false;
        }

        if (name.Length < Constants.DB_MIN_OBJECT_NAME_LENGTH)
            error = $"The name must be at least {Constants.DB_MIN_OBJECT_NAME_LENGTH} characters long";

        if (name.Length > Constants.DB_MAX_OBJECT_NAME_LENGTH)
            error = $"The length of name must be less or equal than {Constants.DB_MAX_OBJECT_NAME_LENGTH} characters";

        Match match = Regex.Match(name, Constants.DB_OBJECT_NAME_VALIDATION_PATTERN);
        if (!match.Success || match.Value != name.Trim())
            error = $"Name can only contains underscores and lowercase alphanumeric characters. It must begin with a letter, " +
                $"not include spaces, not end with an underscore, and not contain two consecutive underscores";

        return string.IsNullOrWhiteSpace(error);
    }

    public static string ConvertDbColumnDefaultValueToDatabaseDefaultValue(TfDatabaseColumn column)
    {
        Func<TfNumberDatabaseColumn, string> numberDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else
                return ((decimal)column.DefaultValue).ToString(CultureInfo.InvariantCulture);
        };

		Func<TfShortIntegerDatabaseColumn, string> shortIntegerDefaultValueFunc = (column) =>
		{
			if (column.DefaultValue is null)
				return "NULL";
			else
				return ((short)column.DefaultValue).ToString();
		};

		Func<TfIntegerDatabaseColumn, string> integerDefaultValueFunc = (column) =>
		{
			if (column.DefaultValue is null)
				return "NULL";
			else
				return ((int)column.DefaultValue).ToString();
		};

		Func<TfLongIntegerDatabaseColumn, string> longIntegerDefaultValueFunc = (column) =>
		{
			if (column.DefaultValue is null)
				return "NULL";
			else
				return ((long)column.DefaultValue).ToString();
		};

		Func<TfBooleanDatabaseColumn, string> booleanDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else if ((bool)column.DefaultValue)
                return "TRUE";
            else
                return "FALSE";
        };

        Func<TfDateDatabaseColumn, string> dateDefaultValueFunc = (column) =>
        {
            if (column.AutoDefaultValue)
                return Constants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE;

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{((DateOnly)column.DefaultValue).ToString("yyyy-MM-dd")}'";
        };

        Func<TfDateTimeDatabaseColumn, string> dateTimeDefaultValueFunc = (column) =>
        {
            if (column.AutoDefaultValue)
                return Constants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE;

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{((DateTime)column.DefaultValue).ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
        };

        Func<TfGuidDatabaseColumn, string> guidDefaultValueFunc = (column) =>
        {
            if (column.AutoDefaultValue)
                return Constants.DB_GUID_COLUMN_AUTO_DEFAULT_VALUE;

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{(Guid)column.DefaultValue}'";
        };

        Func<TfTextDatabaseColumn, string> textDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{column.DefaultValue}'";
        };

		Func<TfShortTextDatabaseColumn, string> shortTextDefaultValueFunc = (column) =>
		{
			if (column.DefaultValue is null)
				return "NULL";
			else
				return $"'{column.DefaultValue}'";
		};

		return column switch
        {
            TfAutoIncrementDatabaseColumn c => null,
            TfNumberDatabaseColumn c => numberDefaultValueFunc(c),
            TfBooleanDatabaseColumn c => booleanDefaultValueFunc(c),
            TfDateDatabaseColumn c => dateDefaultValueFunc(c),
            TfDateTimeDatabaseColumn c => dateTimeDefaultValueFunc(c),
            TfGuidDatabaseColumn c => guidDefaultValueFunc(c),
            TfTextDatabaseColumn c => textDefaultValueFunc(c),
			TfShortTextDatabaseColumn c => shortTextDefaultValueFunc(c),
			TfShortIntegerDatabaseColumn c => shortIntegerDefaultValueFunc(c),
			TfIntegerDatabaseColumn c => integerDefaultValueFunc(c),
			TfLongIntegerDatabaseColumn c => longIntegerDefaultValueFunc(c),
			_ => throw new Exception($"Not supported DbColumn type {column.GetType()} while trying to extract default value")
        };
    }

    public static object ConvertDatabaseDefaultValueToDbColumnDefaultValue(string columnName, Type columnType, string defaultValue)
    {
        if (columnType == typeof(TfAutoIncrementDatabaseColumn))
        {
            return null;
        }
        else if (columnType == typeof(TfNumberDatabaseColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;


			var processedDefaultValue = defaultValue
				.Replace("::numeric", "")
				.Replace("'", "");

			if (decimal.TryParse(processedDefaultValue, CultureInfo.InvariantCulture, out var decimalValue))
                return decimalValue;
        }
        else if (columnType == typeof(TfBooleanDatabaseColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (bool.TryParse(defaultValue, out var boolValue))
                return boolValue;
        }
        else if (columnType == typeof(TfDateDatabaseColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (defaultValue == Constants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE)
                return null;

            var processedDefaultValue = defaultValue
                .Replace("::date", "")
                .Replace("'", "");

            if (DateOnly.TryParse(processedDefaultValue, out var dateValue))
                return dateValue;
        }
        else if (columnType == typeof(TfDateTimeDatabaseColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (defaultValue == Constants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE)
                return null;

            var processedDefaultValue = defaultValue
                .Replace("::timestamp without time zone", "")
                .Replace("'", "");

            if (DateTime.TryParse(processedDefaultValue, out var dateTimeValue))
                return dateTimeValue;
        }
        else if (columnType == typeof(TfGuidDatabaseColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (defaultValue == Constants.DB_GUID_COLUMN_AUTO_DEFAULT_VALUE)
                return null;

            var processedDefaultValue = defaultValue
                .Replace("::uuid", "")
                .Replace("'", "");

            if (Guid.TryParse(processedDefaultValue, out var guidValue))
                return guidValue;
        }
        else if (columnType == typeof(TfTextDatabaseColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            return defaultValue
                 .Replace("::text", "")
                 .Replace("'", "");
        }
		else if (columnType == typeof(TfShortTextDatabaseColumn))
		{
			if (string.IsNullOrWhiteSpace(defaultValue))
				return null;

			return defaultValue
				 .Replace("::character varying", "")
				 .Replace("'", "");
		}
		else if (columnType == typeof(TfShortIntegerDatabaseColumn))
		{
			if (string.IsNullOrWhiteSpace(defaultValue))
				return null;

			if (short.TryParse(defaultValue, out var shortValue))
				return shortValue;
		}
		else if (columnType == typeof(TfIntegerDatabaseColumn))
		{
			if (string.IsNullOrWhiteSpace(defaultValue))
				return null;

			if (int.TryParse(defaultValue, out var intValue))
				return intValue;
		}
		else if (columnType == typeof(TfLongIntegerDatabaseColumn))
		{
			if (string.IsNullOrWhiteSpace(defaultValue))
				return null;

			var processedDefaultValue = defaultValue
				 .Replace("::bigint", "")
				 .Replace("'", "");

			if (long.TryParse(processedDefaultValue, out var longValue))
				return longValue;
		}

		throw new Exception($"Not supported default value \"{defaultValue}\" for column '{columnName}'");
    }
}
