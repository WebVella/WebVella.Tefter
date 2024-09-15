namespace WebVella.Tefter.Web.Store;

public partial class UserStateEffects
{
	[EffectMethod]
	public Task HandleSetUserStateAction(SetUserStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new UserStateChangedAction());
		return Task.CompletedTask;
	}

}

