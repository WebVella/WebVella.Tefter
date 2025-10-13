namespace WebVella.Tefter;

public partial class TfConstants
{
	public static List<TfCultureOption> CultureOptions = new List<TfCultureOption>(){
		new TfCultureOption{ CultureName = "en-US", IconUrl ="/_content/WebVella.Tefter/media/us.svg"},
		new TfCultureOption{ CultureName = "es-ES", IconUrl ="/_content/WebVella.Tefter/media/es.svg"},
		new TfCultureOption{ CultureName = "de-DE", IconUrl ="/_content/WebVella.Tefter/media/de.svg"},
		new TfCultureOption{ CultureName = "fr-FR", IconUrl ="/_content/WebVella.Tefter/media/fr.svg"},
		new TfCultureOption{ CultureName = "bg-BG", IconUrl ="/_content/WebVella.Tefter/media/bg.svg"},
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
