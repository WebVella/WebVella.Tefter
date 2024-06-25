namespace WebVella.Tefter.Web.Models;

public record CultureOption
{
	public string CultureCode { get; set; }
	public CultureInfo CultureInfo { get => CultureInfo.GetCultureInfo(CultureCode); }
	public string IconUrl { get; set; }
	public string Name { get; set; }
}
