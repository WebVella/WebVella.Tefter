namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();

	}
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (TfAppState.Value.Space is not null && TfAppState.Value.SpaceViewList.Count > 0)
		{
			Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceViewList[0].Id));
		}
		else
		{
			StateHasChanged();
		}
	}

	private async Task _createViewHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewManageDialog>(
		new TucSpaceView() with { SpaceId = TfAppState.Value.Space.Id },
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


}