namespace WebVella.Tefter.Web.Utils;

public partial class TfConstants
{
	public static List<CultureOption> CultureOptions = new List<CultureOption>(){
		new CultureOption{ CultureCode = "en-US", IconUrl ="/media/us.svg", Name = "English (US)"},
		new CultureOption{ CultureCode = "bg-BG", IconUrl ="/media/bg.svg", Name = "Български"}
	};

	public static string TimeZoneName = "Europe/Sofia";
	public const string DateFormat = "dd.MM.yyyy";
	public const string HourFormat = "HH:mm";
	public const string DateFormatUrl = "yyyy-MM-dd";
	public const string YearMonthFormatUrl = "yyyy-MM";
	public const string DateHourFormat = "dd MMM yyyy HH:mm";
	public const string DateTimeFormat = "dd MMM yyyy HH:mm:ss";
	public const string NumberFormat = "G0";
}
