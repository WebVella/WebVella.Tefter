namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceData.SpaceDataDetailsNav.TfSpaceDataDetailsNav", "WebVella.Tefter")]
public partial class TfSpaceDataDetailsNav : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private List<TucSpaceView> viewList = new();


	private List<TucMenuItem> _generateMenu()
	{
		var menu = new List<TucMenuItem>();
		var providerId = TfAppState.Value.Route.DataProviderId ?? Guid.Empty;
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.SpaceDataPageUrl, TfAppState.Value.Route.SpaceId, TfAppState.Value.Route.SpaceDataId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Info(),
			Text = LOC("Details")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.SpaceDataViewsPageUrl, TfAppState.Value.Route.SpaceId, TfAppState.Value.Route.SpaceDataId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Text = LOC("Used in Views")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.SpaceDataDataPageUrl, TfAppState.Value.Route.SpaceId, TfAppState.Value.Route.SpaceDataId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Text = LOC("Data")
		});
		return menu;
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
				if (TfAppState.Value.SpaceDataList.Count > 0)
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

			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceData = item,
				SpaceDataList = itemList
			}));
		}
	}
}