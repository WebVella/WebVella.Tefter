namespace WebVella.Tefter.Talk.Addons;

public class TalkApp : ITfApplicationAddon
{
	public const string ID = "27a7703a-8fe8-4363-aee1-64a219d7520e";
	public const string NAME = "Talk Application";
	public const string DESCRIPTION = "Talk Application Description";
	public const string FLUENT_ICON_NAME = "CommentMultiple";
	public Guid Id { get; init;} =  new Guid(ID);
	public string Name { get; init;} =  NAME;
	public string Description { get; init;} =  DESCRIPTION;
	public string FluentIconName { get; init;} =  FLUENT_ICON_NAME;

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<ITalkService, TalkService>();
	}

}
