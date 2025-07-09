namespace WebVella.Tefter.Utility;

internal static class DateTimeUtils
{
	internal static string ToUrlString(DateTime? date)
	{
		if (date is null)
			return null;
		return date.Value.ToString(TfConstants.DateFormatUrl, CultureInfo.InvariantCulture);
	}

	internal static DateTime? FromUrlString(string dateString)
	{
		if (String.IsNullOrWhiteSpace(dateString))
			return null;

		DateTime? result = null;
		if (DateTime.TryParseExact(dateString, TfConstants.DateFormatUrl, CultureInfo.InvariantCulture,
			DateTimeStyles.AssumeLocal, out DateTime outDate))
		{
			result = outDate;
		}
		if (result is null)
			throw new Exception("date string not in proper format");

		return result;
	}
}