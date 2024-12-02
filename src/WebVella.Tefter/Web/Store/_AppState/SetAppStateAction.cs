namespace WebVella.Tefter.Web.Store;

public record SetAppStateAction : TfBaseAction
{
	public TfAppState State { get; }
	internal SetAppStateAction(
		FluxorComponent component,
		TfAppState state
		)
	{
		StateComponent = component;
		State = state;
	}
}

public static partial class AppStateReducers
{
	[ReducerMethod()]
	public static TfAppState SetAppStateActionReducer(TfAppState state, SetAppStateAction action)
	{
		return action.State with { Hash = Guid.NewGuid() };

	}
}

//public partial class AppStateEffect
//{
//	private readonly IJSRuntime _jsRuntime;
//	public AppStateEffect(IJSRuntime jsRuntime)
//	{
//		_jsRuntime = jsRuntime;
//	}
//	[EffectMethod]
//	public async Task HandleSetAppStateActionEffect(SetAppStateAction action,IDispatcher dispatcher)
//	{
//		await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", TfConstants.SpaceViewSelectedRowsKey, JsonSerializer.Serialize(action.State.SelectedDataRows));
//	}
//}