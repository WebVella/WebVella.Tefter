namespace WebVella.Tefter.EmailSender;

public class EmailSenderApp : ITfApplication
{
	public Guid Id => EmailSenderConstants.APP_ID;

	public string Name => EmailSenderConstants.APP_NAME;

	public string Description => EmailSenderConstants.APP_DECRIPTION;
	public string FluentIconName => "Mail";

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<ISmtpConfigurationService, SmtpConfigurationService>();
		services.AddSingleton<ISmtpService, SmtpService>();
		services.AddSingleton<IEmailService, EmailService>();
		services.AddHostedService<SmtpBackgroundJob>();
	}

}
