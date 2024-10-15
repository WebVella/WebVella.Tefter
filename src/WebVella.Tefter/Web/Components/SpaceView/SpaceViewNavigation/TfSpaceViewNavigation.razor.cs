using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewNavigation.TfSpaceViewNavigation", "WebVella.Tefter")]
public partial class TfSpaceViewNavigation : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected ProtectedLocalStorage ProtectedLocalStorage { get; set; }

	private bool _settingsMenuVisible = false;
	private string search = null;
	private TfSpaceViewNavigationActiveTab _activeTab = TfSpaceViewNavigationActiveTab.Views;
	private bool _linksFromAllViews = false;
	private List<string> _expandedGroups = new List<string>();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_expandedGroups = await _getOpenedGroupsFromStorage();
	}
	private List<TucMenuItem> _getMenu()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucMenuItem>();
		var menuGroups = new List<string>();
		if (_activeTab == TfSpaceViewNavigationActiveTab.Views)
		{
			foreach (var record in TfAppState.Value.SpaceViewList.OrderBy(x => x.Name))
			{
				if (!String.IsNullOrWhiteSpace(search) && !record.Name.ToLowerInvariant().Contains(search))
					continue;

				var viewMenu = new TucMenuItem
				{
					Id = TfConverters.ConvertGuidToHtmlElementId(record.Id),
					Icon = TfConstants.SpaceViewIcon,
					Match = NavLinkMatch.Prefix,
					Title = record.Name,
					Url = String.Format(TfConstants.SpaceViewPageUrl, record.SpaceId, record.Id),
					Active = record.Id == TfRouteState.Value.SpaceViewId,
					IsGroup = false,
					Groups = record.Groups
				};
				menuItems.Add(viewMenu);
				menuGroups.AddRange(record.Groups.Where(x => !menuGroups.Contains(x)));
			}

			if (menuGroups.Count > 0)
			{
				var menuWithGroups = new List<TucMenuItem>();
				foreach (var group in menuGroups.Order())
				{
					var viewMenu = new TucMenuItem
					{
						Id = $"tf-{TfAppState.Value.Space.Id}-" + group.Slugify(),
						Icon = TfConstants.GetIcon("Folder"),
						Title = group,
						IsGroup = true,
						Nodes = menuItems.Where(x => x.Groups.Contains(group)).ToList()
					};
					menuWithGroups.Add(viewMenu);
				}

				menuWithGroups.AddRange(menuItems.Where(x => x.Groups is null || !x.Groups.Any()));
				menuItems = menuWithGroups;
			}

		}
		else if (_activeTab == TfSpaceViewNavigationActiveTab.Bookmarks)
		{
			foreach (var record in TfAppState.Value.CurrentUserBookmarks
				.Where(x => x.SpaceId == TfAppState.Value.Space.Id).OrderBy(x => x.Name))
			{
				if (!String.IsNullOrWhiteSpace(search)
					&& !(record.Name.ToLowerInvariant().Contains(search) || record.Description.ToLowerInvariant().Contains(search)))
					continue;

				var viewMenu = new TucMenuItem
				{
					Id = TfConverters.ConvertGuidToHtmlElementId(record.Id),
					Icon = TfConstants.BookmarkOFFIcon,
					Match = NavLinkMatch.Prefix,
					Title = record.Name,
					Url = String.Format(TfConstants.SpaceViewPageUrl, record.SpaceId, record.SpaceViewId),
					Active = record.SpaceViewId == TfRouteState.Value.SpaceViewId
				};
				menuItems.Add(viewMenu);
			}
		}
		else if (_activeTab == TfSpaceViewNavigationActiveTab.Saves)
		{
			foreach (var record in TfAppState.Value.CurrentUserSaves
				.Where(x => x.SpaceId == TfAppState.Value.Space.Id).OrderBy(x => x.Name))
			{
				if (!String.IsNullOrWhiteSpace(search)
					&& !(record.Name.ToLowerInvariant().Contains(search) || record.Description.ToLowerInvariant().Contains(search)))
					continue;

				if (!_linksFromAllViews && record.SpaceViewId != TfAppState.Value.SpaceView.Id) continue;

				var viewMenu = new TucMenuItem
				{
					Id = TfConverters.ConvertGuidToHtmlElementId(record.Id),
					Icon = TfConstants.GetIcon("Link"),
					Match = NavLinkMatch.Prefix,
					Title = record.Name,
					Url = NavigatorExt.AddQueryValueToUri(record.Url, TfConstants.ActiveSaveQueryName, record.Id.ToString()),
					Active = record.Id == TfRouteState.Value.ActiveSaveId
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

	private async Task _groupExpandedChanged(string groupId)
	{
		if (_expandedGroups.Contains(groupId))
		{
			Console.WriteLine("COLLAPSE");
			_expandedGroups.Remove(groupId);
			await _removeOpenedGroupsToStorage(groupId);
		}
		else
		{
			Console.WriteLine("EXPAND");
			_expandedGroups.Add(groupId);
			await _setOpenedGroupsToStorage(groupId);
		}
		await InvokeAsync(StateHasChanged);
	}

	private async Task<List<string>> _getOpenedGroupsFromStorage()
	{
		var result = await ProtectedLocalStorage.GetAsync<List<string>>(TfConstants.SpaceViewOpenedGroupsLocalStorageKey);
		if (result.Success) return result.Value;
		return new List<string>();
	}

	private async Task<List<string>> _setOpenedGroupsToStorage(string groupId)
	{
		var current = await _getOpenedGroupsFromStorage();
		if (!current.Contains(groupId))
		{
			current.Add(groupId);
			await ProtectedLocalStorage.SetAsync(TfConstants.SpaceViewOpenedGroupsLocalStorageKey, current);
		}
		return current;
	}
	private async Task<List<string>> _removeOpenedGroupsToStorage(string groupId)
	{
		var current = await _getOpenedGroupsFromStorage();
		if (current.Contains(groupId))
		{
			current.Remove(groupId);
			await ProtectedLocalStorage.SetAsync(TfConstants.SpaceViewOpenedGroupsLocalStorageKey, current);
		}
		return current;
	}
}

public enum TfSpaceViewNavigationActiveTab
{
	Views = 0,
	Bookmarks = 1,
	Saves = 2
}

