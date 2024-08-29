using WebVella.Tefter.Web.Components.SpaceViewManageDialog;

namespace WebVella.Tefter.Web.Components.SpaceDetails;
public partial class TfSpaceDetails : TfBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	[Inject] private SpaceUseCase UC { get; set; }
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		base.OnInitialized();
		await UC.Init(this.GetType());
		ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_StateChanedResult);
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}

	private void On_StateChanedResult(SpaceStateChangedAction action)
	{
		if(SpaceState.Value.RouteSpaceViewId is null && SpaceState.Value.SpaceViewList.Count > 0){ 
			Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, SpaceState.Value.RouteSpaceId, SpaceState.Value.SpaceViewList[0].Id));
		}

		StateHasChanged();
	}

	private async Task _createViewHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewManageDialog>(
		new TucSpaceView() with { SpaceId = SpaceState.Value.RouteSpaceId.Value },
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpaceView)result.Data;
			ToastService.ShowSuccess(LOC("Space view successfully created!"));
			Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, item.SpaceId, item.Id));
		}
	}


}