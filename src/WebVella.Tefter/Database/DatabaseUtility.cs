namespace WebVella.Tefter.Database;

internal class DatabaseUtility
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

    public static string ConvertDbColumnDefaultValueToDatabaseDefaultValue(DatabaseColumn column)
    {
        Func<NumberDatabaseColumn, string> numberDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else
                return ((decimal)column.DefaultValue).ToString(CultureInfo.InvariantCulture);
        };

		Func<ShortIntegerDatabaseColumn, string> shortIntegerDefaultValueFunc = (column) =>
		{
			if (column.DefaultValue is null)
				return "NULL";
			else
				return ((short)column.DefaultValue).ToString();
		};

		Func<IntegerDatabaseColumn, string> integerDefaultValueFunc = (column) =>
		{
			if (column.DefaultValue is null)
				return "NULL";
			else
				return ((int)column.DefaultValue).ToString();
		};

		Func<LongIntegerDatabaseColumn, string> longIntegerDefaultValueFunc = (column) =>
		{
			if (column.DefaultValue is null)
				return "NULL";
			else
				return ((long)column.DefaultValue).ToString();
		};

		Func<BooleanDatabaseColumn, string> booleanDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else if ((bool)column.DefaultValue)
                return "TRUE";
            else
                return "FALSE";
        };

        Func<DateDatabaseColumn, string> dateDefaultValueFunc = (column) =>
        {
            if (column.AutoDefaultValue)
                return Constants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE;

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{((DateOnly)column.DefaultValue).ToString("yyyy-MM-dd")}'";
        };

        Func<DateTimeDatabaseColumn, string> dateTimeDefaultValueFunc = (column) =>
        {
            if (column.AutoDefaultValue)
                return Constants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE;

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{((DateTime)column.DefaultValue).ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
        };

        Func<GuidDatabaseColumn, string> guidDefaultValueFunc = (column) =>
        {
            if (column.AutoDefaultValue)
                return Constants.DB_GUID_COLUMN_AUTO_DEFAULT_VALUE;

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{(Guid)column.DefaultValue}'";
        };

        Func<TextDatabaseColumn, string> textDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{column.DefaultValue}'";
        };

		Func<ShortTextDatabaseColumn, string> shortTextDefaultValueFunc = (column) =>
		{
			if (column.DefaultValue is null)
				return "NULL";
			else
				return $"'{column.DefaultValue}'";
		};

		return column switch
        {
            AutoIncrementDatabaseColumn c => null,
            NumberDatabaseColumn c => numberDefaultValueFunc(c),
            BooleanDatabaseColumn c => booleanDefaultValueFunc(c),
            DateDatabaseColumn c => dateDefaultValueFunc(c),
            DateTimeDatabaseColumn c => dateTimeDefaultValueFunc(c),
            GuidDatabaseColumn c => guidDefaultValueFunc(c),
            TextDatabaseColumn c => textDefaultValueFunc(c),
			ShortTextDatabaseColumn c => shortTextDefaultValueFunc(c),
			ShortIntegerDatabaseColumn c => shortIntegerDefaultValueFunc(c),
			IntegerDatabaseColumn c => integerDefaultValueFunc(c),
			LongIntegerDatabaseColumn c => longIntegerDefaultValueFunc(c),
			_ => throw new Exception($"Not supported DbColumn type {column.GetType()} while trying to extract default value")
        };
    }

    public static object ConvertDatabaseDefaultValueToDbColumnDefaultValue(string columnName, Type columnType, string defaultValue)
    {
        if (columnType == typeof(AutoIncrementDatabaseColumn))
        {
            return null;
        }
        else if (columnType == typeof(NumberDatabaseColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (decimal.TryParse(defaultValue, CultureInfo.InvariantCulture, out var decimalValue))
                return decimalValue;
        }
        else if (columnType == typeof(BooleanDatabaseColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (bool.TryParse(defaultValue, out var boolValue))
                return boolValue;
        }
        else if (columnType == typeof(DateDatabaseColumn))
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
        else if (columnType == typeof(DateTimeDatabaseColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (defaultValue == Constants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE)
                return null;

            var processedDefaultValue = defaultValue
                .Replace("::timestamp with time zone", "")
                .Replace("'", "");

            if (DateTime.TryParse(processedDefaultValue, out var dateTimeValue))
                return dateTimeValue;
        }
        else if (columnType == typeof(GuidDatabaseColumn))
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
        else if (columnType == typeof(TextDatabaseColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            return defaultValue
                 .Replace("::text", "")
                 .Replace("'", "");
        }
		else if (columnType == typeof(ShortTextDatabaseColumn))
		{
			if (string.IsNullOrWhiteSpace(defaultValue))
				return null;

			return defaultValue
				 .Replace("::character varying", "")
				 .Replace("'", "");
		}
		else if (columnType == typeof(ShortIntegerDatabaseColumn))
		{
			if (string.IsNullOrWhiteSpace(defaultValue))
				return null;

			if (short.TryParse(defaultValue, out var shortValue))
				return shortValue;
		}
		else if (columnType == typeof(IntegerDatabaseColumn))
		{
			if (string.IsNullOrWhiteSpace(defaultValue))
				return null;

			if (int.TryParse(defaultValue, out var intValue))
				return intValue;
		}
		else if (columnType == typeof(LongIntegerDatabaseColumn))
		{
			if (string.IsNullOrWhiteSpace(defaultValue))
				return null;

			if (long.TryParse(defaultValue, out var longValue))
				return longValue;
		}

		throw new Exception($"Not supported default value \"{defaultValue}\" for column '{columnName}'");
    }
}
