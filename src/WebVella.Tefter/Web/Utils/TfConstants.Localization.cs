namespace WebVella.Tefter.Web.Utils;

public partial class TfConstants
{
	public static List<TucCultureOption> CultureOptions = new List<TucCultureOption>(){
		new TucCultureOption{ CultureCode = "en-US", IconUrl ="/_content/WebVella.Tefter/media/us.svg", Name = "English (US)"},
		new TucCultureOption{ CultureCode = "bg-BG", IconUrl ="/_content/WebVella.Tefter/media/bg.svg", Name = "Български"}
	};

	public static string TimeZoneName = "Europe/Sofia";
	public const string DateFormat = "dd.MM.yyyy";
	public const string HourFormat = "HH:mm";
	public const string DateFormatUrl = "yyyy-MM-dd";
	public const string YearMonthFormatUrl = "yyyy-MM";
	public const string DateHourFormat = "dd MMM yyyy HH:mm";
	public const string DateTimeFormat = "dd MMM yyyy HH:mm:ss";
	public const string NumberFormat = "G0";
	public const string DateOnlyFormatInput = "yyyy-MM-dd";
	public const string DateTimeFormatInput = "yyyy-MM-dd HH:mm";
}
