namespace WebVella.Tefter.Email;

public class EmailApp : ITfApplication
{
	public Guid Id => TfEmailConstants.APP_ID;

	public string Name => TfEmailConstants.APP_NAME;

	public string Description => TfEmailConstants.APP_DECRIPTION;

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
