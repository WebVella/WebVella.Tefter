namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task SetUserDetailsActionEffect(SetUserDetailsAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new GetUserDetailsActionResult(action.User));
		return Task.CompletedTask;
	}
}