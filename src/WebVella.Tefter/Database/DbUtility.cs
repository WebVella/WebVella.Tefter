using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace WebVella.Tefter.Database;

class DbUtility
{
    public static string ConvertDbColumnDefaultValueToDatabaseDefaultValue(DbColumn column)
    {
        Func<DbNumberColumn, string> numberDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else
                return ((decimal)column.DefaultValue).ToString();
        };

        Func<DbBooleanColumn, string> booleanDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else if ((bool)column.DefaultValue)
                return "TRUE";
            else
                return "FALSE";
        };

        Func<DbDateColumn, string> dateDefaultValueFunc = (column) =>
        {
            if (column.UseCurrentTimeAsDefaultValue)
                return "now()";

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{((DateOnly)column.DefaultValue).ToString("yyyy-MM-dd")}'";
        };

        Func<DbDateTimeColumn, string> dateTimeDefaultValueFunc = (column) =>
        {
            if (column.UseCurrentTimeAsDefaultValue)
                return "now()";

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{((DateTime)column.DefaultValue).ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
        };

        Func<DbGuidColumn, string> guidDefaultValueFunc = (column) =>
        {
            if (column.GenerateNewIdAsDefaultValue)
                return "uuid_generate_v1()";

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{(Guid)column.DefaultValue}'";
        };

        Func<DbTextColumn, string> textDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{column.DefaultValue}'";
        };

        Func<DbTableIdColumn, string> tableIdDefaultValueFunc = (column) =>
        {
            return "uuid_generate_v1()";
        };

        return column switch
        {
            DbTableIdColumn c => tableIdDefaultValueFunc(c),
            DbAutoIncrementColumn c => null,
            DbNumberColumn c => numberDefaultValueFunc(c),
            DbBooleanColumn c => booleanDefaultValueFunc(c),
            DbDateColumn c => dateDefaultValueFunc(c),
            DbDateTimeColumn c => dateTimeDefaultValueFunc(c),
            DbGuidColumn c => guidDefaultValueFunc(c),
            DbTextColumn c => textDefaultValueFunc(c),
            _ => throw new Exception($"Not supported DbColumn type {column.GetType()} while trying to extract default value")
        };
    }

    public static object ConvertDatabaseDefaultValueToDbColumnDefaultValue(string columnName, Type columnType, string defaultValue)
    {
        if (columnType == typeof(DbAutoIncrementColumn))
        {
            return null;
        }
        else if (columnType == typeof(DbTableIdColumn))
        {
            return null;
        }
        else if (columnType == typeof(DbNumberColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (decimal.TryParse(defaultValue, out var decimalValue))
                return decimalValue;
        }
        else if (columnType == typeof(DbBooleanColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (bool.TryParse(defaultValue, out var boolValue))
                return boolValue;
        }
        else if (columnType == typeof(DbDateColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (defaultValue == "now()")
                return null;

            var processedDefaultValue = defaultValue
                .Replace("::date", "")
                .Replace("'", "");

            if (DateOnly.TryParse(processedDefaultValue, out var dateValue))
                return dateValue;
        }
        else if (columnType == typeof(DbDateTimeColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (defaultValue == "now()")
                return null;

            var processedDefaultValue = defaultValue
                .Replace("::timestamp with time zone", "")
                .Replace("'", "");

            if (DateTime.TryParse(processedDefaultValue, out var dateTimeValue))
                return dateTimeValue;
        }
        else if (columnType == typeof(DbGuidColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

            if (defaultValue == "uuid_generate_v1()")
                return null;

            var processedDefaultValue = defaultValue
                .Replace("::uuid", "")
                .Replace("'", "");

            if (Guid.TryParse(processedDefaultValue, out var guidValue))
                return guidValue;
        }
        else if (columnType == typeof(DbTextColumn))
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                return null;

           return defaultValue
                .Replace("::text", "")
                .Replace("'", "");
        }

        throw new Exception($"Not supported default value \"{defaultValue}\" for column '{columnName}'");
    }
}
