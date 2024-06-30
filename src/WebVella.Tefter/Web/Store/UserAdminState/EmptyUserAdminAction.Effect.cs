namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task EmptyUserAdminActionEffect(EmptyUserAdminAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new UserAdminChangedAction(null));
		return Task.CompletedTask;
	}
}