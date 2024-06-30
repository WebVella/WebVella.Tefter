namespace WebVella.Tefter.UseCases.Models;

public record TucCultureOption
{
	public string CultureCode { get; set; }
	public CultureInfo CultureInfo { get => CultureInfo.GetCultureInfo(CultureCode); }
	public string IconUrl { get; set; }
	public string Name { get; set; }
}
