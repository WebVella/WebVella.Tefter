namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceDataDetails : TfBaseComponent
{
	[Parameter] public string Menu { get; set; } = "";
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }

}