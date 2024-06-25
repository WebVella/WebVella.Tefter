namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task EmptyUserDetailsActionEffect(EmptyUserDetailsAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new UserDetailsChangedAction(null));
		return Task.CompletedTask;
	}
}