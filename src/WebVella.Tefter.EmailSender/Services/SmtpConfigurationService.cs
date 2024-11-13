
namespace WebVella.Tefter.EmailSender.Services;

internal interface ISmtpConfigurationService
{
	bool Enabled { get; }
	string Server { get; }
	int Port { get; }
	string Username { get; }
	string Password { get; }

	string DefaultSenderName { get; }
	string DefaultSenderEmail { get; }
	string DefaultReplyToEmail { get; }
}


internal class SmtpConfigurationService : ISmtpConfigurationService
{
	public bool Enabled { get; } = false;
	public string Server { get; } = "localhost";
	public int Port { get; } = 25;
	public string Username { get; } = null;
	public string Password { get; } = null;
	public string DefaultSenderName { get; } = null;
	public string DefaultSenderEmail { get; } = null;

	public string DefaultReplyToEmail { get; } = null;

	public SmtpConfigurationService(ITfConfigurationService tfConfiguration )
	{
		var config = tfConfiguration.Config;

		if (bool.TryParse(config["Tefter:Email:Smtp:Enabled"], out var enabled)) Enabled = enabled;
		if (int.TryParse(config["Tefter:Email:Smtp:Port"], out var port)) Port = port;

		Server = !string.IsNullOrEmpty(config["Tefter:Email:Smtp:Server"]) ? config["Tefter:Email:Smtp:Server"] : Server;
		Username = !string.IsNullOrEmpty(config["Tefter:Email:Smtp:Username"]) ? config["Tefter:Email:Smtp:Username"] : null;
		Password = !string.IsNullOrEmpty(config["Tefter:Email:Smtp:Password"]) ? config["Tefter:Email:Smtp:Password"] : null;
		DefaultSenderName = !string.IsNullOrEmpty(config["Tefter:Email:Smtp:DefaultSenderName"]) ? config["Tefter:Email:Smtp:DefaultSenderName"] : null;
		DefaultSenderEmail = !string.IsNullOrEmpty(config["Tefter:Email:Smtp:DefaultSenderEmail"]) ? config["Tefter:Email:Smtp:DefaultSenderEmail"] : null;
		DefaultReplyToEmail = !string.IsNullOrEmpty(config["Tefter:Email:Smtp:DefaultReplyToEmail"]) ? config["Tefter:Email:Smtp:DefaultReplyToEmail"] : null;

	}
}
