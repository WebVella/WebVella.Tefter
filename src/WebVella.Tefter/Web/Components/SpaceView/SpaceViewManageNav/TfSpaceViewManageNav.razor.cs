namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewManageNav.TfSpaceViewManageNav", "WebVella.Tefter")]
public partial class TfSpaceViewManageNav : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }

	
	private List<TucMenuItem> _generateMenu()
	{
		List<TucMenuItem> menu = new();
		var providerId = Navigator.GetRouteState().DataProviderId ?? Guid.Empty;
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.SpaceViewManagePageUrl, TfRouteState.Value.SpaceId, TfRouteState.Value.SpaceViewId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Info(),
			Title = LOC("View Management")
		});
		return menu;
	}

	private async Task _editSpaceView()
	{

		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewManageDialog>(
		TfAppState.Value.SpaceView,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var resultObj = (Tuple<TucSpaceView,TucSpaceData>)result.Data;
			var spaceView = resultObj.Item1;
			var spaceData = resultObj.Item2;
			var viewList = TfAppState.Value.SpaceViewList.ToList();
			var dataList = TfAppState.Value.SpaceDataList.ToList();
			var viewIndex = viewList.FindIndex(x => x.Id == spaceView.Id);
			if (viewIndex > -1)
			{
				viewList[viewIndex] = spaceView;
			}

			var dataIndex = dataList.FindIndex(x => x.Id == spaceData.Id);
			if (dataIndex > -1)
			{
				dataList[dataIndex] = spaceData;
			}
			else{ 
				dataList.Add(spaceData);
			}

			Dispatcher.Dispatch(new SetAppStateAction(
						component: this,
						state: TfAppState.Value with
						{
							SpaceView = spaceView,
							SpaceViewList = viewList.OrderBy(x=> x.Position).ToList(),
							SpaceDataList = dataList.OrderBy(x=> x.Position).ToList()
						}));

			ToastService.ShowSuccess(LOC("Space view successfully updated!"));
		}
	}
}