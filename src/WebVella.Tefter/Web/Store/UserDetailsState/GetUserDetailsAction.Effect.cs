namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public async Task GetUserDetailsActionEffect(GetUserDetailsAction action, IDispatcher dispatcher)
	{
		if(action.RecordId == Guid.Empty) return;
		User user = null;
		var result = await IdentityManager.GetUserAsync(action.RecordId);
		if (result.IsSuccess) user = result.Value;
		dispatcher.Dispatch(new UserDetailsChangedAction(user));
	}
}