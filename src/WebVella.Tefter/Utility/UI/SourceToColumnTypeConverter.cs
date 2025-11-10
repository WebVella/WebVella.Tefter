namespace WebVella.Tefter.Utility;

public static class SourceToColumnTypeConverter
{
	public static TfDatabaseColumnType GetDataTypeFromString(string str, CultureInfo culture, string? importFormat = null)
	{

		bool boolValue;
		decimal decimalValue;
		short shortValue;
		Int32 intValue;
		Int64 bigintValue;
		DateOnly dateOnlyValue;
		DateTime dateTimeValue;
		Guid guidValue;

		// Place checks higher in if-else statement to give higher priority to type.

		if (short.TryParse(str, out shortValue)) return TfDatabaseColumnType.ShortInteger;
		else if (Int32.TryParse(str, out intValue)) return TfDatabaseColumnType.Integer;
		else if (Int64.TryParse(str, out bigintValue)) return TfDatabaseColumnType.LongInteger;
		else if (Guid.TryParse(str, out guidValue)) return TfDatabaseColumnType.Guid;
		else if (bool.TryParse(str, out boolValue)) return TfDatabaseColumnType.Boolean;
		else if (decimal.TryParse(str, culture, out decimalValue)) return TfDatabaseColumnType.Number;
		else if (DateOnly.TryParseExact(str, importFormat, culture, DateTimeStyles.None, out dateOnlyValue)) return TfDatabaseColumnType.DateOnly;
		else if (DateOnly.TryParse(str, culture, out dateOnlyValue)) return TfDatabaseColumnType.DateOnly;
		else if (DateTime.TryParseExact(str, importFormat, culture, DateTimeStyles.None, out dateTimeValue)) return TfDatabaseColumnType.DateOnly;
		else if (DateTime.TryParse(str, culture, out dateTimeValue)) return TfDatabaseColumnType.DateTime;
		else if (str.Length <= 4000) return TfDatabaseColumnType.ShortText;
		else return TfDatabaseColumnType.Text;

	}

	public static TfDatabaseColumnType GetTypeFromOptions(this List<TfDatabaseColumnType> options)
	{
		if (options.Count == 0) return TfDatabaseColumnType.Text;
		var distinctOptions = options.Distinct().ToList();
		if (distinctOptions.Count == 1) return distinctOptions[0];

		if (distinctOptions.Contains(TfDatabaseColumnType.Text)) return TfDatabaseColumnType.Text;
		if (distinctOptions.Contains(TfDatabaseColumnType.ShortText)) return TfDatabaseColumnType.ShortText;
		if (distinctOptions.Contains(TfDatabaseColumnType.Guid)) return TfDatabaseColumnType.Guid;
		if (distinctOptions.Contains(TfDatabaseColumnType.Number)) return TfDatabaseColumnType.Number;
		if (distinctOptions.Contains(TfDatabaseColumnType.DateTime)) return TfDatabaseColumnType.DateTime;
		if (distinctOptions.Contains(TfDatabaseColumnType.DateOnly)) return TfDatabaseColumnType.DateOnly;
		if (distinctOptions.Contains(TfDatabaseColumnType.Boolean)) return TfDatabaseColumnType.Boolean;
		if (distinctOptions.Contains(TfDatabaseColumnType.LongInteger)) return TfDatabaseColumnType.LongInteger;
		if (distinctOptions.Contains(TfDatabaseColumnType.Integer)) return TfDatabaseColumnType.Integer;
		if (distinctOptions.Contains(TfDatabaseColumnType.ShortInteger)) return TfDatabaseColumnType.ShortInteger;
		return TfDatabaseColumnType.Text;

	}

	public static HashSet<int> GenerateSampleIndexesForList(this int itemsCount, int maxSampleSize, int? skipCount = null)
	{
		var result = new HashSet<int>();
		if(itemsCount <= 0) return result;
		
		int totalItems = itemsCount;
		int startIndex = 0;
		if (skipCount is not null && skipCount.Value > 0)
		{
			totalItems = totalItems - skipCount.Value;
			startIndex = skipCount.Value;
		}

		if (maxSampleSize >= totalItems)
		{
			for (int i = startIndex; i < totalItems; i++)
			{
				result.Add(i);
			}
		}
		else
		{
			for (int i = startIndex; i < (maxSampleSize / 2); i++)
			{
				result.Add(i);
			}
			for (int i = totalItems - 1; i >= (totalItems - maxSampleSize / 2); i--)
			{
				result.Add(i);
			}
		}


		return result;
	}

	public static string ToSourceColumnName(this string sourceColumnName)
	 => Regex.Replace(sourceColumnName, @"[^\p{C}]+", m => m.Value);
}
