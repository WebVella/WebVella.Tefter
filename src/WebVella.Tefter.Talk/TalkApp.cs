namespace WebVella.Tefter.Talk;

public class TalkApp : ITfApplication
{
	public Guid Id => TfTalkConstants.TALK_APP_ID;

	public string Name => TfTalkConstants.TALK_APP_NAME;

	public string Description => TfTalkConstants.TALK_APP_DECRIPTION;
	public string FluentIconName => "CommentMultiple";

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<ITalkService, TalkService>();
	}

}
