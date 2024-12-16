namespace WebVella.Tefter.Talk;

public class TalkApp : ITfApplication
{
	public Guid Id => TalkConstants.TALK_APP_ID;

	public string Name => TalkConstants.TALK_APP_NAME;

	public string Description => TalkConstants.TALK_APP_DECRIPTION;
	public string FluentIconName => "CommentMultiple";

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<ITalkService, TalkService>();
	}

}
