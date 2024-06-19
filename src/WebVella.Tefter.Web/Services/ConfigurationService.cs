namespace WebVella.Tefter.Web.Services;

public interface IConfigurationService
{
	public string CompanyName { get; }
	public string CompanyLogoUrl { get; }
}

public class ConfigurationService : IConfigurationService
{
	public string CompanyName { get; }
	public string CompanyLogoUrl { get; }

	private IConfiguration configuration;

	public ConfigurationService(IConfiguration config)
	{
		configuration = config;
		var companyNameConfigString = config["Tefter:CompanyName"];
		CompanyName = !string.IsNullOrWhiteSpace(companyNameConfigString) ? companyNameConfigString : "Tefter.bg";
		var companyLogoUrlConfigString = config["Tefter:CompanyLogoUrl"];
		CompanyLogoUrl = !string.IsNullOrWhiteSpace(companyLogoUrlConfigString) ? companyLogoUrlConfigString : "/logo.svg";
	}
}