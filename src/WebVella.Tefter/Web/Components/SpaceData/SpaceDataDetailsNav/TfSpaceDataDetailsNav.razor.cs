namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceDataDetailsNav.TfSpaceDataDetailsNav", "WebVella.Tefter")]
public partial class TfSpaceDataDetailsNav : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private List<MenuItem> menu = new();
	private List<TucSpaceView> viewList = new();

	private TfRouteState _urlData = new();

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		GenerateMenu();
		ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_StateChanged);
		Navigator.LocationChanged += Navigator_LocationChanged;
		StateHasChanged();
	}

	private void GenerateMenu()
	{
		menu.Clear();
		var providerId = Navigator.GetRouteState().DataProviderId ?? Guid.Empty;
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.SpaceDataPageUrl, TfRouteState.Value.SpaceId, TfRouteState.Value.SpaceDataId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Info(),
			Title = LOC("Details")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.SpaceDataPageUrl, TfRouteState.Value.SpaceId, TfRouteState.Value.SpaceDataId) + "\\views",
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Title = LOC("Used in Views")
		});
		_urlData = Navigator.GetRouteState();
	}


	private void On_StateChanged(SpaceStateChangedAction action)
	{
		GenerateMenu();
		StateHasChanged();
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		GenerateMenu();
		StateHasChanged();
	}

	private async Task _deleteSpaceData()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this dataset deleted?")))
			return;
		try
		{

			Result result = UC.DeleteSpaceData(TfAppState.Value.SpaceData.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space data deleted"));
				if(TfAppState.Value.SpaceDataList.Count > 0)
					Navigator.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceDataList[0].Id), true);
				else
					Navigator.NavigateTo(String.Format(TfConstants.SpacePageUrl, TfAppState.Value.Space.Id), true);
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}

	}
	private async Task _editSpaceData()
	{

		var dialog = await DialogService.ShowDialogAsync<TfSpaceDataManageDialog>(
		TfAppState.Value.SpaceData,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpaceData)result.Data;
			var itemList = TfAppState.Value.SpaceDataList.ToList();
			var itemIndex = itemList.FindIndex(x => x.Id == item.Id);
			if (itemIndex > -1)
			{
				itemList[itemIndex] = item;
			}

			ToastService.ShowSuccess(LOC("Space dataset successfully updated!"));


			Dispatcher.Dispatch(new SetSpaceDataAction(
						component: this,
						spaceData: item,
						spaceDataList: itemList));
		}
	}
}