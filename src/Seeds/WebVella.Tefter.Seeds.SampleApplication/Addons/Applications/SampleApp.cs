namespace WebVella.Tefter.Seeds.SampleApplication;

public class SampleApp : ITfApplicationAddon
{
	public Guid Id { get; init;} =  SampleAppConstants.APP_ID;

	public string Name { get; init;} =  SampleAppConstants.APP_NAME;

	public string Description { get; init;} =  SampleAppConstants.APP_DECRIPTION;
	public string FluentIconName { get; init;} =  "Album";

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<ISampleAppService, SampleAppService>();
	}

}