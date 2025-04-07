namespace WebVella.Tefter.Seeds.SampleApplication;

public class SampleApp : ITfApplication
{
	public Guid Id => SampleAppConstants.APP_ID;

	public string Name => SampleAppConstants.APP_NAME;

	public string Description => SampleAppConstants.APP_DECRIPTION;
	public string FluentIconName => "Album";

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<ISampleAppService, SampleAppService>();
	}

}