namespace WebVella.Tefter.Models;
public class TfCultureOption
{
	public string CultureName { get; set; }
	[JsonIgnore]
	public CultureInfo CultureInfo { get => CultureInfo.GetCultureInfo(CultureName); }
	public string IconUrl { get; set; }
}
