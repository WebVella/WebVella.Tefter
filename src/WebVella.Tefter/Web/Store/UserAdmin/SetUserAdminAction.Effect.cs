namespace WebVella.Tefter.Web.Store;

public partial class TfStateEffect
{
	[EffectMethod]
	public Task SetUserAdminActionEffect(SetUserAdminAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new UserAdminChangedAction(action.User));
		return Task.CompletedTask;
	}
}