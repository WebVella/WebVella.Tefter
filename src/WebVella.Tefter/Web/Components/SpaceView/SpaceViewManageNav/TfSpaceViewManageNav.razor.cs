namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceViewManageNav.TfSpaceViewManageNav", "WebVella.Tefter")]
public partial class TfSpaceViewManageNav : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }

	
	private List<MenuItem> _generateMenu()
	{
		List<MenuItem> menu = new();
		var providerId = Navigator.GetRouteState().DataProviderId ?? Guid.Empty;
		menu.Add(new MenuItem
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
			var item = (TucSpaceView)result.Data;
			var itemList = TfAppState.Value.SpaceViewList.ToList();
			var itemIndex = itemList.FindIndex(x => x.Id == item.Id);
			if (itemIndex > -1)
			{
				itemList[itemIndex] = item;
			}

			ToastService.ShowSuccess(LOC("Space view successfully updated!"));


			Dispatcher.Dispatch(new SetAppStateAction(
						component: this,
						state: TfAppState.Value with
						{
							SpaceView = item,
							SpaceViewList = itemList
						}));
		}
	}
}