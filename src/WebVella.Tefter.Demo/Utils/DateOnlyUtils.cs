namespace WebVella.Tefter.Demo.Utils;

public static class DateOnlyUtils
{
	public static string ToString(DateOnly? date) => date?.ToString(WvConstants.DateFormatUrl);
	public static string ForView(DateOnly? date) => date?.ToString(WvConstants.DateFormat, WvConstants.Culture);
	public static string FromStringForView(string dateString)
	{
		var date = FromString(dateString);
		return ForView(date);
	}
	public static DateOnly? FromString(string dateString)
	{
		if (string.IsNullOrWhiteSpace(dateString))
			return null;

		if (DateOnly.TryParseExact(dateString, WvConstants.DateFormatUrl, out DateOnly outDate))
			return outDate;

		throw new Exception($"Date only string \"{dateString}\" not in proper format: {WvConstants.DateFormatUrl}");
	}

	public static string ToUrlString(DateOnly? date)
	{
		if (date is null)
			return null;
		return date.Value.ToString(WvConstants.DateFormatUrl, CultureInfo.InvariantCulture);
	}

	public static DateOnly? FromUrlString(string dateString)
	{
		if (String.IsNullOrWhiteSpace(dateString))
			return null;

		DateOnly? result = null;
		if (DateOnly.TryParseExact(dateString, WvConstants.DateFormatUrl, CultureInfo.InvariantCulture,
			DateTimeStyles.AssumeLocal, out DateOnly outDate))
		{
			result = outDate;
		}
		if (result is null)
			throw new Exception("date string not in proper format");

		return result;
	}
}
