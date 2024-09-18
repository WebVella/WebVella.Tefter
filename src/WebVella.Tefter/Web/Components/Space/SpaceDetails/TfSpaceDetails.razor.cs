namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }

	private bool visible = false;
	private int counterINT = 0;
	private int counter = 0;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		counterINT++;
		Console.WriteLine($"+****************** OnInitialized {counterINT} {TfRouteState.Value.SpaceId} {TfAppState.Value.SpaceViewList.Count}");
	}
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (TfRouteState.Value.SpaceId is not null && TfAppState.Value.SpaceViewList.Count > 0)
		{
			counter++;
			Console.WriteLine($"+****************** NavigateTo {counter}");
			Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, TfRouteState.Value.SpaceId, TfAppState.Value.SpaceViewList[0].Id));
		}
		else
		{
			visible = true;
			StateHasChanged();
		}
	}

	private async Task _createViewHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewManageDialog>(
		new TucSpaceView() with { SpaceId = TfRouteState.Value.SpaceId.Value },
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