namespace WebVella.Tefter.Web.Services;

public interface IConfigurationService
{
	public string CompanyName { get; }
}

public class ConfigurationService : IConfigurationService
{
	public string CompanyName { get; }

	private IConfiguration configuration;

	public ConfigurationService(IConfiguration config)
	{
		configuration = config;
		var companyNameConfigString = config["Tefter:CompanyName"];
		CompanyName = !string.IsNullOrWhiteSpace(companyNameConfigString) ? companyNameConfigString : "Tefter.bg";
	}
}