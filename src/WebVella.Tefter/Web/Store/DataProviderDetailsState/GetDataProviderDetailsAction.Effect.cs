namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task GetDataProviderDetailsActionEffect(GetDataProviderDetailsAction action, IDispatcher dispatcher)
	{
		if(action.RecordId == Guid.Empty) return Task.CompletedTask;
		TfDataProvider provider = null;
		var result = DataPrividerManager.GetProvider(action.RecordId);
		if (result.IsSuccess) provider = result.Value;
		dispatcher.Dispatch(new DataProviderDetailsChangedAction(provider));
		return Task.CompletedTask;
	}
}