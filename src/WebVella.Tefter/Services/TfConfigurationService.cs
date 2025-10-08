namespace WebVella.Tefter;

public interface ITfConfigurationService
{
	public IConfiguration Config { get; }
	public string ConnectionString { get; }
	public string BlobStoragePath { get; }
	public string CryptoPassword { get; }
	public string CompanyName { get; }
	public string CompanySlogan { get; }
	public string CompanyLogoUrl { get; }
	public string BaseUrl { get; }
	public string CacheKey { get; }
	public string Version { get; }



}

public class TfConfigurationService : ITfConfigurationService
{
	public IConfiguration Config { get; init; }
	public string ConnectionString { get; init; }
	public string BlobStoragePath { get; }
	public string CryptoPassword { get; init; }
	public string CompanyName { get; init; }
	public string CompanySlogan { get; init; }
	public string CompanyLogoUrl { get; init; }
	public string BaseUrl { get; init; }
	public string CacheKey { get; init; }
	public string Version { get; init; }

	public TfConfigurationService(IConfiguration config)
	{
		Config = config;
		ConnectionString = config["Tefter:ConnectionString"];
		BlobStoragePath = config["Tefter:BlobStoragePath"];
		CryptoPassword = config["Tefter:CryptoPassword"];
		CompanyName = config["Tefter:CompanyName"];
		CompanySlogan = config["Tefter:CompanySlogan"];
		CompanyLogoUrl = config["Tefter:CompanyLogoUrl"];
		BaseUrl = config["Tefter:BaseUrl"];
		CacheKey = config["Tefter:CacheKey"];
		if (String.IsNullOrWhiteSpace(CacheKey))
			CacheKey = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
		if (string.IsNullOrWhiteSpace(CompanyName)) CompanyName = "Tefter";
		if (string.IsNullOrWhiteSpace(CompanySlogan)) CompanySlogan = "by WebVella";
		if (string.IsNullOrWhiteSpace(CompanyLogoUrl)) CompanyLogoUrl = "_content/WebVella.Tefter/logo.svg";

		Version = config["Tefter:Version"];
	}
}
