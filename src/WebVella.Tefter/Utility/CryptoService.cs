namespace WebVella.Tefter.Utility;

public interface ICryptoUtilityService
{
	string Encrypt(string plainText);
	string Decrypt(string cipherText);
}
public class CryptoUtilityService : ICryptoUtilityService
{
	private const string defaultPass = "Keys are created with a default lifetime of 90 days";
	private readonly ICryptoServiceConfiguration _options;
	private readonly IDataProtectionProvider _provider;
	private string _password;

	public CryptoUtilityService(
		ICryptoServiceConfiguration cryptoServiceConfiguration,
		IDataProtectionProvider provider
		)
	{
		_options = cryptoServiceConfiguration;
		_provider = provider;
		if(string.IsNullOrWhiteSpace(_options.Password))
			_password = defaultPass;
		else
			_password = _options.Password;
	}

	public string Encrypt(string plainText)
	{
		var protector = _provider.CreateProtector(_password);
		return protector.Protect(plainText);
	}

	public string Decrypt(string cipherText)
	{
		var protector = _provider.CreateProtector(_password);
		return protector.Unprotect(cipherText);
	}
}
