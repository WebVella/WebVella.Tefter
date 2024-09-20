namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewNavigation.TfSpaceViewNavigation", "WebVella.Tefter")]
public partial class TfSpaceViewNavigation : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private bool _settingsMenuVisible = false;
	private string search = null;


	private List<MenuItem> _getMenu()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<MenuItem>();
		foreach (var view in TfAppState.Value.SpaceViewList)
		{
			if (!String.IsNullOrWhiteSpace(search) && !view.Name.ToLowerInvariant().Contains(search))
				continue;

			var viewMenu = new MenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(view.Id),
				Icon = TfConstants.SpaceViewIcon,
				Match = NavLinkMatch.Prefix,
				Title = view.Name,
				Url = String.Format(TfConstants.SpaceViewPageUrl, view.SpaceId, view.Id),
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
			var item = (TucSpaceView)result.Data;
			ToastService.ShowSuccess(LOC("Space view successfully created!"));
			Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, item.SpaceId, item.Id));
		}
	}

	private async Task onManageSpaceClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceManageDialog>(
		TfAppState.Value.Space,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpace)result.Data;
			ToastService.ShowSuccess(LOC("Space successfully updated!"));
			//Change user state > spaces
			var userSpaces = TfAppState.Value.CurrentUserSpaces.ToList();
			var itemIndex = userSpaces.FindIndex(x => x.Id == item.Id);
			if (itemIndex > -1)
			{
				userSpaces[itemIndex] = item;
			}
			var state = TfAppState.Value with { CurrentUserSpaces = userSpaces };
			if (TfAppState.Value.Space is not null
				&& TfAppState.Value.Space.Id == item.Id)
			{
				state = state with { Space = item };
			}

			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: state
			));
		}
	}

	private void onDataListClick()
	{
		Guid? spaceDataId = null;
		if (TfAppState.Value.SpaceDataList.Count > 0) spaceDataId = TfAppState.Value.SpaceDataList[0].Id;
		Navigator.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl, TfAppState.Value.Space.Id, spaceDataId));
	}
	private void onSearch(string value)
	{
		search = value;
	}
}