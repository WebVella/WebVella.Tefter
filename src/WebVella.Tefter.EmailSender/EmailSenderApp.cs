namespace WebVella.Tefter.EmailSender;

public class EmailSenderApp : ITfApplicationAddon
{
	public Guid Id { get; init; } = EmailSenderConstants.APP_ID;

	public string Name { get; init; } = EmailSenderConstants.APP_NAME;

	public string Description { get; init; } = EmailSenderConstants.APP_DECRIPTION;
	public string FluentIconName { get; init; } = "Mail";

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
