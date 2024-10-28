using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewNavigation.TfSpaceViewNavigation", "WebVella.Tefter")]
public partial class TfSpaceViewNavigation : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected ProtectedLocalStorage ProtectedLocalStorage { get; set; }

	[Inject] private AppStateUseCase UC { get; set; }

	private bool _settingsMenuVisible = false;
	private string search = null;

	private List<TucMenuItem> _getMenu()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucMenuItem>();
		var menuGroups = new List<string>();

		foreach (var record in TfAppState.Value.SpaceViewList.OrderBy(x => x.Name))
		{
			if (!String.IsNullOrWhiteSpace(search) && !record.Name.ToLowerInvariant().Contains(search))
				continue;

			var viewMenu = new TucMenuItem
			{
				Id = TfConverters.ConvertGuidToHtmlElementId(record.Id),
				IconCollapsed = TfConstants.SpaceViewIcon,
				Text = record.Name,
				Url = String.Format(TfConstants.SpaceViewPageUrl, record.SpaceId, record.Id),
				Selected = record.Id == TfRouteState.Value.SpaceViewId
			};
			menuItems.Add(viewMenu);
		}

		return menuItems;
	}

	private async Task onAddClick()
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
			var resultObj = (Tuple<TucSpaceView, TucSpaceData>)result.Data;
			var spaceView = resultObj.Item1;
			var spaceData = resultObj.Item2;
			var viewList = TfAppState.Value.SpaceViewList.ToList();
			var dataList = TfAppState.Value.SpaceDataList.ToList();
			viewList.Add(spaceView);

			var dataIndex = dataList.FindIndex(x => x.Id == spaceData.Id);
			if (dataIndex > -1)
			{
				dataList[dataIndex] = spaceData;
			}
			else
			{
				dataList.Add(spaceData);
			}

			Dispatcher.Dispatch(new SetAppStateAction(
						component: this,
						state: TfAppState.Value with
						{
							SpaceView = spaceView,
							SpaceViewList = viewList.OrderBy(x => x.Position).ToList(),
							SpaceDataList = dataList.OrderBy(x => x.Position).ToList()
						}));

			ToastService.ShowSuccess(LOC("Space view successfully created!"));
			Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, spaceView.SpaceId, spaceView.Id));
		}
	}
	private async Task onDeleteSpaceClick()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space deleted?")))
			return;

		try
		{
			var result = UC.DeleteSpace(TfAppState.Value.Space.Id);

			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				var spaceList = TfAppState.Value.CurrentUserSpaces.Where(x => x.Id != TfAppState.Value.Space.Id).ToList();
				Dispatcher.Dispatch(new SetAppStateAction(
									component: this,
									state: TfAppState.Value with
									{
										CurrentUserSpaces = spaceList
									}
								));
				Navigator.NavigateTo(TfConstants.HomePageUrl);
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
	private void onManageSpaceClick()
	{
		Navigator.NavigateTo(String.Format(TfConstants.SpaceManagePageUrl, TfAppState.Value.Space.Id));
	}

	private void onDataListClick()
	{
		Guid? spaceDataId = null;
		if (TfAppState.Value.SpaceDataList.Count > 0) spaceDataId = TfAppState.Value.SpaceDataList[0].Id;
		Navigator.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl, TfAppState.Value.Space.Id, spaceDataId));
	}
	private void onViewListClick()
	{
		Guid? spaceViewId = null;
		if (TfAppState.Value.SpaceViewList.Count > 0) spaceViewId = TfAppState.Value.SpaceViewList[0].Id;
		Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, TfAppState.Value.Space.Id, spaceViewId));
	}

	private void onPageListClick()
	{
		Navigator.NavigateTo(String.Format(TfConstants.SpaceNodePageUrl, TfAppState.Value.Space.Id, TfAppState.Value.Space.DefaultNodeId));
	}

	private void onSearch(string value)
	{
		search = value;
	}

}
