namespace WebVella.Tefter.Web.Store;

public partial class TfStateEffect
{
	[EffectMethod]
	public Task EmptyUserAdminActionEffect(EmptyUserAdminAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new UserAdminChangedAction(action.User));
		return Task.CompletedTask;
	}
}