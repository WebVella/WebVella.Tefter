namespace WebVella.Tefter;

public interface ITfConfigurationService
{
	public IConfiguration Config { get; }
	public string ConnectionString { get; }
	public string FilesRootPath { get; }
	public string CryptoPassword { get; }
	public string CompanyName { get; }
	public string CompanyLogoUrl { get; }

	

}

public class TfConfigurationService : ITfConfigurationService
{
	public IConfiguration Config { get; init; }
	public string ConnectionString { get; init; }
	public string FilesRootPath { get; init; }
	public string CryptoPassword { get; init; }
	public string CompanyName { get; init; }
	public string CompanyLogoUrl { get; init; }

	public TfConfigurationService(IConfiguration config)
	{
		Config = config;
		ConnectionString = config["Tefter:ConnectionString"];
		FilesRootPath = config["Tefter:FilesRootPath"];
		CryptoPassword = config["Tefter:CryptoPassword"];
		CompanyName = config["Tefter:CompanyName"];
		CompanyLogoUrl = config["Tefter:CompanyLogoUrl"];
	}
}
