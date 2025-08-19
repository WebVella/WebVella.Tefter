namespace WebVella.Tefter.Models;
public class TfCultureOption
{
	public string CultureCode { get; set; }
	[JsonIgnore]
	public CultureInfo CultureInfo { get => CultureInfo.GetCultureInfo(CultureCode); }
	public string IconUrl { get; set; }
}
