namespace WebVella.Tefter.Templates;

public class TemplatesApp : ITfApplication
{
	public Guid Id => TfTemplatesConstants.TEMPLATES_APP_ID;

	public string Name => TfTemplatesConstants.TEMPLATES_APP_NAME;

	public string Description => TfTemplatesConstants.TEMPLATES_APP_DECRIPTION;

	public string FluentIconName => string.Empty;

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<ITemplatesService, TemplatesService>();
	}

}
