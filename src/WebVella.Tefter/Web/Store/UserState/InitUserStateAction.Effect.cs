namespace WebVella.Tefter.Web.Store.UserState;

public partial class UserStateEffects
{
	[EffectMethod]
	public Task HandleInitUserStateAction(InitUserStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SetUserActionResult());
		return Task.CompletedTask;
	}

}

