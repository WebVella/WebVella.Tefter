namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task SetUserAdminActionEffect(SetUserAdminAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new UserAdminChangedAction(action.IsBusy,action.User));
		return Task.CompletedTask;
	}
}