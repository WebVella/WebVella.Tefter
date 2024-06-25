namespace WebVella.Tefter.Web.Store.UserState;

public partial class UserStateEffects
{
	[EffectMethod]
	public Task HandleSetUserAction(SetUserAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SetUserActionResult());
		return Task.CompletedTask;
	}

}

