namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceDataNavigation : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private bool _settingsMenuVisible = false;
	private string search = null;

	private List<MenuItem> _getMenu()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<MenuItem>();
		foreach (var spaceData in TfAppState.Value.SpaceDataList.OrderBy(x=> x.Name))
		{
			if (!String.IsNullOrWhiteSpace(search) && !spaceData.Name.ToLowerInvariant().Contains(search))
				continue;

			var menu = new MenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(spaceData.Id),
				Icon = TfConstants.SpaceDataIcon,
				Match = NavLinkMatch.Prefix,
				Title = spaceData.Name,
				Url = String.Format(TfConstants.SpaceDataPageUrl, spaceData.SpaceId, spaceData.Id),
			};
			menuItems.Add(menu);
		}

		return menuItems;
	}

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceDataManageDialog>(
		new TucSpaceData { SpaceId = TfAppState.Value.Space.Id },
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
			ToastService.ShowSuccess(LOC("Space dataset successfully created!"));

			var itemList = TfAppState.Value.SpaceDataList.ToList();
			itemList.Add(item);

			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceData = item,
				SpaceDataList = itemList.OrderBy(x => x.Position).ToList()
			}));

			Navigator.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl, item.SpaceId, item.Id));
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

	private void onViewsListClick()
	{
		Guid? spaceViewId = null;
		if (TfAppState.Value.SpaceViewList.Count > 0) spaceViewId = TfAppState.Value.SpaceViewList[0].Id;
		Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, TfAppState.Value.Space.Id, spaceViewId));
	}

	private void onSearch(string value)
	{
		search = value;
	}
}