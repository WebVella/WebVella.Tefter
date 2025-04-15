namespace WebVella.Tefter;

public interface ITfConfigurationService
{
	public IConfiguration Config { get; }
	public string ConnectionString { get; }
	//TODO Rumen - remove
	public string FilesRootPath { get; }
	public string BlobStoragePath { get; }
	public string CryptoPassword { get; }
	public string CompanyName { get; }
	public string CompanyLogoUrl { get; }
	public string BaseUrl { get; }



}

public class TfConfigurationService : ITfConfigurationService
{
	public IConfiguration Config { get; init; }
	public string ConnectionString { get; init; }
	//TODO Rumen - remove
	public string FilesRootPath { get; init; }
	public string BlobStoragePath { get; }
	public string CryptoPassword { get; init; }
	public string CompanyName { get; init; }
	public string CompanyLogoUrl { get; init; }
	public string BaseUrl { get; init; }

	public TfConfigurationService(IConfiguration config)
	{
		Config = config;
		ConnectionString = config["Tefter:ConnectionString"];
		FilesRootPath = config["Tefter:FilesRootPath"];
		BlobStoragePath = config["Tefter:BlobStoragePath"];
		CryptoPassword = config["Tefter:CryptoPassword"];
		CompanyName = config["Tefter:CompanyName"];
		CompanyLogoUrl = config["Tefter:CompanyLogoUrl"];
		BaseUrl = config["Tefter:BaseUrl"];
		if (string.IsNullOrWhiteSpace(CompanyName)) CompanyName = "Tefter by WebVella";
		if (string.IsNullOrWhiteSpace(CompanyLogoUrl)) CompanyLogoUrl = "_content/WebVella.Tefter/logo.svg";
	}
}
