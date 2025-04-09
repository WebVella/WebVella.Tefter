namespace WebVella.Tefter.Talk;

public class TalkApp : ITfApplicationAddon
{
	public Guid Id { get; init;} =  TalkConstants.TALK_APP_ID;

	public string Name { get; init;} =  TalkConstants.TALK_APP_NAME;

	public string Description { get; init;} =  TalkConstants.TALK_APP_DECRIPTION;
	public string FluentIconName { get; init;} =  "CommentMultiple";

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<ITalkService, TalkService>();
	}

}
