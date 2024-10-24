namespace WebVella.Tefter.Web.Components;
public partial class TfSpacePageDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();

	}

}