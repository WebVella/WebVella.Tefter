namespace WebVella.Tefter.Talk;

public class TalkApp : ITfApplication
{
	public Guid Id => Constants.TALK_APP_ID;

	public string Name => Constants.TALK_APP_NAME;

	public string Description => Constants.TALK_APP_DECRIPTION;

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<ITalkService, TalkService>();
	}

}
