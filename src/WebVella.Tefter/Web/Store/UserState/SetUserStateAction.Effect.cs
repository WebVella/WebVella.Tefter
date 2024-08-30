namespace WebVella.Tefter.Web.Store.UserState;

public partial class UserStateEffects
{
	[EffectMethod]
	public Task HandleSetUserStateAction(SetUserStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new UserStateChangedAction());
		return Task.CompletedTask;
	}

}

