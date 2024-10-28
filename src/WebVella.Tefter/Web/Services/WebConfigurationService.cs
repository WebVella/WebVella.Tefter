namespace WebVella.Tefter.Web.Services;

public interface IWebConfigurationService
{
	public string CompanyName { get; }
	public string CompanyLogoUrl { get; }
}

public class WebConfigurationService : IWebConfigurationService
{
	public string CompanyName { get; }
	public string CompanyLogoUrl { get; }

	private ITfConfigurationService configuration;

	public WebConfigurationService(ITfConfigurationService config)
	{
		configuration = config;
		var companyNameConfigString = config.CompanyName;
		CompanyName = !string.IsNullOrWhiteSpace(companyNameConfigString) ? companyNameConfigString : "Tefter.bg";
		var companyLogoUrlConfigString = config.CompanyLogoUrl;
		CompanyLogoUrl = !string.IsNullOrWhiteSpace(companyLogoUrlConfigString) ? companyLogoUrlConfigString : "_content/WebVella.Tefter/logo.svg";
	}
}