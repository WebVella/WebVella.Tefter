namespace WebVella.Tefter.Seeds.SampleApplication;

public class SampleApp : ITfApplicationAddon
{
	public const string ID = "1492d19b-c8b2-4abb-9e88-2b645ba518ff";
	public const string NAME = "Sample Application";
	public const string DESCRIPTION = "Sample Application Description";
	public const string FLUENT_ICON_NAME = "Album";

	public Guid Id { get; init;} =  new Guid(ID);
	public string Name { get; init;} =  NAME;
	public string Description { get; init;} =  DESCRIPTION;
	public string FluentIconName { get; init;} =  FLUENT_ICON_NAME;

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<ISampleAppService, SampleAppService>();
	}
}