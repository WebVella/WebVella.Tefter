namespace WebVella.Tefter.Database;

public interface ICryptoServiceConfiguration
{
    public string Password { get;  }
}

public class CryptoServiceConfiguration : ICryptoServiceConfiguration
{
    public string Password { get; private set; }

    public CryptoServiceConfiguration(IConfiguration config)
    {
		Password = config["Tefter:CryptoPassword"];
    }
}
