namespace WebVella.Tefter;

public partial class TfConstants
{
	public static List<TucCultureOption> CultureOptions = new List<TucCultureOption>(){
		new TucCultureOption{ CultureCode = "en-US", IconUrl ="/_content/WebVella.Tefter/media/us.svg", Name = "English (US)"},
		new TucCultureOption{ CultureCode = "bg-BG", IconUrl ="/_content/WebVella.Tefter/media/bg.svg", Name = "Български"}
	};
	public static CultureInfo DefaultCulture = new CultureInfo("en-US");
	public static string TimeZoneName = "Europe/Sofia";
	public const string DateFormat = "yyyy-MM-dd";
	public const string HourFormat = "HH:mm";
	public const string DateHourFormat = "yyyy-MM-dd HH:mm";
	public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
	public const string DateTimeFullFormat = "yyyy-MM-dd HH:mm:ss.fff";
	public const string NumberFormat = "G0";
	public const string DateOnlyFormatInput = "yyyy-MM-dd";
	public const string DateTimeFormatInput = "yyyy-MM-dd HH:mm";

	public const string DateFormatUrl = "yyyy-MM-dd";
	public const string DateTimeFormatUrl = "yyyy-MM-dd HH:mm:ss";
	public const string YearMonthFormatUrl = "yyyy-MM";
}
