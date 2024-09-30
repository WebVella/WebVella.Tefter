using WebVella.Tefter.Talk.Services;

namespace WebVella.Tefter.Talk;

public class TalkApp : TfApplicationBase
{
	public override Guid Id => Constants.TALK_APP_ID;

	public override string Name => Constants.TALK_APP_NAME;

	public override string Description => Constants.TALK_APP_DECRIPTION;

	public override void OnStart()
	{
		base.OnStart();
	}

	public override void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<ITalkService, TalkService>();

		base.OnRegisterDependencyInjections(services);
	}

}
