using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
namespace WebVella.Tefter.UI.Components;
public partial class TucSpacePageAsideContent : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	[Inject] protected ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private TfSpaceNavigationActiveTab _activeTab = TfSpaceNavigationActiveTab.Pages;
	private List<string> _expandedNodeIdList = new();
	private List<TfMenuItem> _items = new();
	private TfNavigationState _navState = new();
	public void Dispose()
	{
		TfEventProvider?.Dispose();
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		TfEventProvider.SpacePageCreatedEvent += On_SpacePageChanged;
		TfEventProvider.SpacePageUpdatedEvent += On_SpacePageChanged;
		TfEventProvider.SpacePageDeletedEvent += On_SpacePageChanged;
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private async Task On_SpacePageChanged(object args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.GetState().NavigationState);
		});
	}
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;
		try
		{
			_items = new();
			if (_navState.SpaceId is null)
				return;

			_search = _navState.SearchAside;
			_activeTab = NavigatorExt.GetEnumFromQuery<TfSpaceNavigationActiveTab>(Navigator, TfConstants.TabQueryName, TfSpaceNavigationActiveTab.Pages)!.Value;
			await _generateMenu();
		}
		finally
		{
			_isLoading = false;
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _addPage()
	{
		if (_navState.SpaceId == null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
		new TfSpacePage() { SpaceId = _navState.SpaceId.Value },
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TfSpacePage)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.SpacePagePageUrl, _navState.SpaceId.Value, item.Id));
		}

	}

	#region << Menu General >>
	private async Task _generateMenu()
	{
		_expandedNodeIdList = await _getExpandedNodesFromStorage();
		var menuItems = new List<TfMenuItem>();
		var menuGroups = new List<string>();
		if (_activeTab == TfSpaceNavigationActiveTab.Pages)
		{
			var spacePages = TfService.GetSpacePages(_navState.SpaceId!.Value);
			foreach (var spacePage in spacePages)
			{
				if (!_filterPage(spacePage))
					continue;
				var item = spacePage.ToMenuItem((x) =>
				{
					_assignMenuItemActions(x);
				}, _filterPage);

				menuItems.Add(item);
			}
			menuItems.ForEach(x => _setFlags(x));
		}
		else if (_activeTab == TfSpaceNavigationActiveTab.Bookmarks)
		{
			var bookmarks = TfService.GetBookmarksListForUser(TfAuthLayout.GetState().User.Id);

			//TODO - spaceId was removed from spaceView - fix this
			//var spaceViewDict = (TfService.GetSpaceViewsList(_navState.SpaceId!.Value) ?? new List<TfSpaceView>()).ToDictionary(x => x.Id);
			var spaceViewDict = new List<TfSpaceView>().ToDictionary(x => x.Id);

			foreach (var record in bookmarks
				.Where(x => spaceViewDict.ContainsKey(x.SpaceId)).OrderBy(x => x.Name))
			{
				if (!String.IsNullOrWhiteSpace(_search)
					&& !(record.Name.ToLowerInvariant().Contains(_search) || record.Description.ToLowerInvariant().Contains(_search)))
					continue;
				var url = string.Format(TfConstants.SpaceViewPageUrl, _navState.SpaceId, record.SpaceId);
				url = NavigatorExt.AddQueryValueToUri(url, TfConstants.TabQueryName, ((int)TfSpaceNavigationActiveTab.Bookmarks).ToString());
				var viewMenu = new TfMenuItem
				{
					Id = TfConverters.ConvertGuidToHtmlElementId(record.Id),
					IconCollapsed = TfConstants.GetIcon("Bookmark"),
					Text = record.Name,
					Url = url,
					Selected = record.SpaceId == _navState.SpaceViewId
				};
				menuItems.Add(viewMenu);
			}
		}
		else if (_activeTab == TfSpaceNavigationActiveTab.Saves)
		{
			var saves = TfService.GetSavesListForUser(TfAuthLayout.GetState().User.Id);
			
			//TODO - spaceId was removed from spaceView - fix this
			//var spaceViewDict = (TfService.GetSpaceViewsList(_navState.SpaceId!.Value) ?? new List<TfSpaceView>()).ToDictionary(x => x.Id);
			var spaceViewDict = new List<TfSpaceView>().ToDictionary(x => x.Id);

			foreach (var record in saves
				.Where(x => spaceViewDict.ContainsKey(x.SpaceId)).OrderBy(x => x.Name))
			{
				if (!String.IsNullOrWhiteSpace(_search)
					&& !(record.Name.ToLowerInvariant().Contains(_search) || record.Description.ToLowerInvariant().Contains(_search)))
					continue;

				var url = NavigatorExt.AddQueryValueToUri(record.Url, TfConstants.ActiveSaveQueryName, record.Id.ToString());
				url = NavigatorExt.AddQueryValueToUri(url, TfConstants.TabQueryName, ((int)TfSpaceNavigationActiveTab.Saves).ToString());
				var viewMenu = new TfMenuItem
				{
					Id = TfConverters.ConvertGuidToHtmlElementId(record.Id),
					IconCollapsed = TfConstants.GetIcon("Link"),
					Text = record.Name,
					Url = url,
					Selected = record.Id == _navState.ActiveSaveId
				};
				menuItems.Add(viewMenu);
			}
		}
		_items = menuItems;
	}

	private void _assignMenuItemActions(TfMenuItem item)
	{
		item.OnClick = EventCallback.Factory.Create(this, async () => await _onMenuItemClick(item));
		item.OnExpand = EventCallback.Factory.Create<bool>(this, async (bool expanded) => await _onMenuItemExpand(item));
	}

	private bool _filterPage(TfSpacePage page)
	{
		var search = _search?.Trim().ToLowerInvariant();
		if (!String.IsNullOrWhiteSpace(search) && !page.Name.ToLowerInvariant().Contains(search))
			return false;

		return true;
	}

	private void _setFlags(TfMenuItem item)
	{
		if (!String.IsNullOrWhiteSpace(item.Id))
			item.Expanded = _expandedNodeIdList.Contains(item.Id!);
		if (_navState.SpacePageId is not null)
			item.Selected = item.IdTree.Contains(_navState.SpacePageId.Value.ToString());

		foreach (var childItem in item.Items ?? new List<TfMenuItem>())
		{
			_setFlags(childItem);
		}
	}

	private async Task _onMenuItemExpand(TfMenuItem item)
	{
		if (item.Expanded)
		{
			await _addExpandedNodesToStorage(item.Id);
		}
		else
		{
			await _removeExpandedNodesFromStorage(item.Id);
		}

	}

	private async Task _onMenuItemClick(TfMenuItem item)
	{
		if (item.Data is not null)
		{
			if (item.Data.SpacePageType == TfSpacePageType.Folder)
			{
				await _onMenuItemExpand(item);
				return;
			}
		}
		if (!String.IsNullOrWhiteSpace(item.Url))
		{
			Navigator.NavigateTo(item.Url);
		}
		else
		{
			item.Selected = !item.Selected;
		}
		return;
	}

	#endregion

	#region << Expanded Local Storage>>

	private async Task<List<string>> _getExpandedNodesFromStorage()
	{
		try
		{
			var result = await ProtectedLocalStorage.GetAsync<List<string>>(TfConstants.SpaceViewOpenedGroupsLocalStorageKey);
			if (result.Success) return result.Value;
			return new List<string>();
		}
		catch
		{
			//if decryption fail delete Protected LocalStoravge
			await ProtectedLocalStorage.DeleteAsync(TfConstants.SpaceViewOpenedGroupsLocalStorageKey);
			return new List<string>();
		}

	}

	private async Task<List<string>> _addExpandedNodesToStorage(string itemId)
	{
		var current = await _getExpandedNodesFromStorage();
		if (!current.Contains(itemId))
		{
			current.Add(itemId);
			await ProtectedLocalStorage.SetAsync(TfConstants.SpaceViewOpenedGroupsLocalStorageKey, current);
		}
		return current;
	}
	private async Task<List<string>> _removeExpandedNodesFromStorage(string itemId)
	{
		var current = await _getExpandedNodesFromStorage();
		if (current.Contains(itemId))
		{
			current.Remove(itemId);
			await ProtectedLocalStorage.SetAsync(TfConstants.SpaceViewOpenedGroupsLocalStorageKey, current);
		}
		return current;
	}
	#endregion

}