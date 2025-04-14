namespace WebVella.Tefter.EmailSender.Addons;

public class EmailSenderApp : ITfApplicationAddon
{
	public const string ID = "0c847f54-28c0-4314-9151-bb9226d42033";
	public const string NAME = "Email Sender Application";
	public const string DESCRIPTION = "Email Sender Application Description";
	public const string FLUENT_ICON_NAME = "Mail";
	public Guid Id { get; init;} =  new Guid(ID);
	public string Name { get; init;} =  NAME;
	public string Description { get; init;} =  DESCRIPTION;
	public string FluentIconName { get; init;} =  FLUENT_ICON_NAME;

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
