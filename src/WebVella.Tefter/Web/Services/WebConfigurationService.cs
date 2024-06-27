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

	private IConfiguration configuration;

	public WebConfigurationService(IConfiguration config)
	{
		configuration = config;
		var companyNameConfigString = config["Tefter:CompanyName"];
		CompanyName = !string.IsNullOrWhiteSpace(companyNameConfigString) ? companyNameConfigString : "Tefter.bg";
		var companyLogoUrlConfigString = config["Tefter:CompanyLogoUrl"];
		CompanyLogoUrl = !string.IsNullOrWhiteSpace(companyLogoUrlConfigString) ? companyLogoUrlConfigString : "_content/WebVella.Tefter/logo.svg";
	}
}