using Fluxor.Blazor.Web.Middlewares.Routing;

namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public async Task GetUserAdminActionEffect(GetUserAdminAction action, IDispatcher dispatcher)
	{
		if(action.RecordId == Guid.Empty) return;
		TucUser user = null;
		throw new NotImplementedException();
		//var result = await IdentityManager.GetUserAsync(action.RecordId);
		//if (result.IsSuccess) user = result.Value;
		//if(user is null) { 
		//	dispatcher.Dispatch(new GoAction(TfConstants.NotFoundPageUrl, true));
		//	return;
		//}

		dispatcher.Dispatch(new UserAdminChangedAction(user));
	}
}