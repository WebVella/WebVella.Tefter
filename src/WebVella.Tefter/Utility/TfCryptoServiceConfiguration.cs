namespace WebVella.Tefter.Database;

public interface ITfCryptoServiceConfiguration
{
	public string Password { get; }
}

public class TfCryptoServiceConfiguration : ITfCryptoServiceConfiguration
{
	public string Password { get; private set; }

	public TfCryptoServiceConfiguration(ITfConfigurationService config)
	{
		Password = config.CryptoPassword;
	}
}
