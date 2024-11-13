namespace WebVella.Tefter.EmailSender;

public class EmailSenderApp : ITfApplication
{
	public Guid Id => TfEmailSenderConstants.APP_ID;

	public string Name => TfEmailSenderConstants.APP_NAME;

	public string Description => TfEmailSenderConstants.APP_DECRIPTION;

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
