namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewNavigation.TfSpaceViewNavigation", "WebVella.Tefter")]
public partial class TfSpaceViewNavigation : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private bool _settingsMenuVisible = false;
	private string search = null;
	private TfSpaceViewNavigationActiveTab _activeTab = TfSpaceViewNavigationActiveTab.Views;
	private FluentButton _viewsBtn;
	private FluentButton _bookmarksBtn;
	private FluentButton _savesBtn;
	private bool _isLoading = false;
	private List<MenuItem> _getMenu()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<MenuItem>();
		if (_activeTab == TfSpaceViewNavigationActiveTab.Views)
		{
			foreach (var record in TfAppState.Value.SpaceViewList.OrderBy(x => x.Name))
			{
				if (!String.IsNullOrWhiteSpace(search) && !record.Name.ToLowerInvariant().Contains(search))
					continue;

				var viewMenu = new MenuItem
				{
					Id = RenderUtils.ConvertGuidToHtmlElementId(record.Id),
					Icon = TfConstants.SpaceViewIcon,
					Match = NavLinkMatch.Prefix,
					Title = record.Name,
					Url = String.Format(TfConstants.SpaceViewPageUrl, record.SpaceId, record.Id),
				};
				menuItems.Add(viewMenu);
			}
		}
		else if (_activeTab == TfSpaceViewNavigationActiveTab.Bookmarks)
		{
			foreach (var record in TfAppState.Value.CurrentUserBookmarks
				.Where(x => x.SpaceId == TfAppState.Value.Space.Id).OrderBy(x => x.Name))
			{
				if (!String.IsNullOrWhiteSpace(search) && !record.Name.ToLowerInvariant().Contains(search))
					continue;

				var viewMenu = new MenuItem
				{
					Id = RenderUtils.ConvertGuidToHtmlElementId(record.Id),
					Icon = TfConstants.BookmarkOFFIcon,
					Match = NavLinkMatch.Prefix,
					Title = record.Name,
					Url = String.Format(TfConstants.SpaceViewPageUrl, record.SpaceId, record.SpaceViewId),
				};
				menuItems.Add(viewMenu);
			}
		}
		else if (_activeTab == TfSpaceViewNavigationActiveTab.Saves)
		{
			foreach (var record in TfAppState.Value.CurrentUserSaves
				.Where(x => x.SpaceId == TfAppState.Value.Space.Id).OrderBy(x => x.Name))
			{
				if (!String.IsNullOrWhiteSpace(search) && !record.Name.ToLowerInvariant().Contains(search))
					continue;

				var uri = new Uri($"http://localhost{record.Url}");
				var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
				queryDictionary[TfConstants.ActiveSaveQueryName] = record.Id.ToString();
				var viewMenu = new MenuItem
				{
					Id = RenderUtils.ConvertGuidToHtmlElementId(record.Id),
					Icon = TfConstants.GetIcon("Link"),
					Match = NavLinkMatch.Prefix,
					Title = record.Name,
					Url = uri.LocalPath + "?" + queryDictionary.ToString(),
				};
				menuItems.Add(viewMenu);
			}
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

	private void _setActiveTab(TfSpaceViewNavigationActiveTab tab)
	{
		if (_activeTab == tab) return;
		_activeTab = tab;
		_getMenu();
	}

	private string _getActiveClass(TfSpaceViewNavigationActiveTab tab)
	{
		return _activeTab == tab ? "active" : "";
	}
}

public enum TfSpaceViewNavigationActiveTab
{
	Views = 0,
	Bookmarks = 1,
	Saves = 2
}

