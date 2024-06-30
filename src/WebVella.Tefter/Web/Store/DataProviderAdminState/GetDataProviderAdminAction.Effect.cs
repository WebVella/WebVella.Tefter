using Fluxor.Blazor.Web.Middlewares.Routing;

namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task GetDataProviderAdminActionEffect(GetDataProviderAdminAction action, IDispatcher dispatcher)
	{
		if(action.RecordId == Guid.Empty) return Task.CompletedTask;
		TfDataProvider provider = null;
		throw new NotImplementedException();
		//var result = DataPrividerManager.GetProvider(action.RecordId);
		//if (result.IsSuccess) provider = result.Value;
		//if (provider is null)
		//{
		//	dispatcher.Dispatch(new GoAction(TfConstants.NotFoundPageUrl, true));
		//	return Task.CompletedTask;
		//}

		//dispatcher.Dispatch(new DataProviderAdminChangedAction(provider));
		//return Task.CompletedTask;
	}
}