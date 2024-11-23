namespace WebVella.Tefter.Web.Utils;
public static class CsvSourceToColumnTypeConverter
{
	public static TfDatabaseColumnType GetDataTypeFromString(string str, CultureInfo culture)
	{

		bool boolValue;
		decimal decimalValue;
		short shortValue;
		Int32 intValue;
		Int64 bigintValue;
		DateOnly dateOnlyValue;
		DateTime dateValue;
		Guid guidValue;

		// Place checks higher in if-else statement to give higher priority to type.

		if (bool.TryParse(str, out boolValue)) return TfDatabaseColumnType.Boolean;
		else if (DateOnly.TryParse(str, culture, out dateOnlyValue)) return TfDatabaseColumnType.Date;
		else if (DateTime.TryParse(str, culture, out dateValue)) return TfDatabaseColumnType.DateTime;
		else if (Guid.TryParse(str, out guidValue)) return TfDatabaseColumnType.Guid;
		else if (decimal.TryParse(str, culture, out decimalValue)) return TfDatabaseColumnType.Number;
		else if (short.TryParse(str, out shortValue)) return TfDatabaseColumnType.ShortInteger;
		else if (Int32.TryParse(str, out intValue)) return TfDatabaseColumnType.Integer;
		else if (Int64.TryParse(str, out bigintValue)) return TfDatabaseColumnType.LongInteger;
		else if (str.Length <= 4000) return TfDatabaseColumnType.ShortText;
		else return TfDatabaseColumnType.Text;

	}

	public static TfDatabaseColumnType GetTypeFromOptions(List<TfDatabaseColumnType> options)
	{
		if(options.Contains(TfDatabaseColumnType.Text)) return TfDatabaseColumnType.Text;
		if(options.Contains(TfDatabaseColumnType.ShortText)) return TfDatabaseColumnType.ShortText;
		if(options.Contains(TfDatabaseColumnType.Guid)) return TfDatabaseColumnType.Guid;
		if(options.Contains(TfDatabaseColumnType.DateTime)) return TfDatabaseColumnType.DateTime;
		if(options.Contains(TfDatabaseColumnType.Date)) return TfDatabaseColumnType.Date;
		if(options.Contains(TfDatabaseColumnType.Boolean)) return TfDatabaseColumnType.Boolean;
		if(options.Contains(TfDatabaseColumnType.Number)) return TfDatabaseColumnType.Number;
		if(options.Contains(TfDatabaseColumnType.LongInteger)) return TfDatabaseColumnType.LongInteger;
		if(options.Contains(TfDatabaseColumnType.Integer)) return TfDatabaseColumnType.Integer;
		if(options.Contains(TfDatabaseColumnType.ShortInteger)) return TfDatabaseColumnType.ShortInteger;
		return TfDatabaseColumnType.Text;

	}
}
