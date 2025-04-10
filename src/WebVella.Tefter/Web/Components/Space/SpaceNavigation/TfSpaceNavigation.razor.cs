using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using WebVella.Tefter.Web.Utils;
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Space.SpaceNavigation.TfSpaceNavigation", "WebVella.Tefter")]
public partial class TfSpaceNavigation : TfBaseComponent
{
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected ProtectedLocalStorage ProtectedLocalStorage { get; set; }
	private List<TucMenuItem> _menu { get; set; } = new();
	private List<string> _expandedNodeIdList = new();
	private string search = null;
	private TfSpaceNavigationActiveTab _activeTab = TfSpaceNavigationActiveTab.Nodes;
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		SidebarExpanded.Select(x => x.SidebarExpanded);
		await _generateMenu();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			ActionSubscriber.SubscribeToAction<SetUserStateAction>(this, On_UserChanged);
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
		}
	}

	private void On_AppChanged(SetAppStateAction action)
	{
		if (action.Component == this) return;
		InvokeAsync(async () =>
		{
			await _generateMenu();
			await InvokeAsync(StateHasChanged);
		});
	}

	private void On_UserChanged(SetUserStateAction action)
	{
		if (action.Component == this) return;
		InvokeAsync(async () =>
		{
			await InvokeAsync(StateHasChanged);
		});
	}

	#region << Toolbar >>
	private string _getTabActiveClass(TfSpaceNavigationActiveTab tab)
	{
		return _activeTab == tab ? "active" : "";
	}
	public enum TfSpaceNavigationActiveTab
	{
		Nodes = 0,
		Bookmarks = 1,
		Saves = 2
	}
	private async Task _setActiveTab(TfSpaceNavigationActiveTab tab)
	{
		if (_activeTab == tab) return;
		_activeTab = tab;
		await _generateMenu();
	}

	private async Task _addPage()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceNodeManageDialog>(
		new TucSpaceNode(),
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
			var nodes = (List<TucSpaceNode>)result.Data;
			ToastService.ShowSuccess(LOC("Space page successfully created!"));
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceNodes = nodes
			}
			));
		}

	}


	private async Task onSearch(string value)
	{
		search = value;
		await _generateMenu();
	}

	

	#endregion

	#region << Menu General >>
	private async Task _generateMenu()
	{
		_expandedNodeIdList = await _getExpandedNodesFromStorage();
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucMenuItem>();
		var menuGroups = new List<string>();
		if (_activeTab == TfSpaceNavigationActiveTab.Nodes)
		{
			foreach (var spaceNode in TfAppState.Value.SpaceNodes)
			{
				var item = spaceNode.ToMenuItem((x) =>
				{
					_assignMenuItemActions(x);
					x.Expanded = _expandedNodeIdList.Contains(x.Id);
					x.Selected = TfAppState.Value.Route.SpaceNodeId == ((TucSpaceNode)x.Data).Id;
				});
				menuItems.Add(item);
			}
		}
		else if (_activeTab == TfSpaceNavigationActiveTab.Bookmarks)
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
					IconCollapsed = TfConstants.BookmarkOFFIcon,
					Match = NavLinkMatch.Prefix,
					Text = record.Name,
					Url = string.Format(TfConstants.SpaceViewPageUrl, record.SpaceId, record.SpaceViewId),
					Selected = record.SpaceViewId == TfAppState.Value.Route.SpaceViewId
				};
				menuItems.Add(viewMenu);
			}
		}
		else if (_activeTab == TfSpaceNavigationActiveTab.Saves)
		{
			foreach (var record in TfAppState.Value.CurrentUserSaves
				.Where(x => x.SpaceId == TfAppState.Value.Space.Id).OrderBy(x => x.Name))
			{
				if (!String.IsNullOrWhiteSpace(search)
					&& !(record.Name.ToLowerInvariant().Contains(search) || record.Description.ToLowerInvariant().Contains(search)))
					continue;


				var viewMenu = new TucMenuItem
				{
					Id = TfConverters.ConvertGuidToHtmlElementId(record.Id),
					IconCollapsed = TfConstants.GetIcon("Link"),
					Match = NavLinkMatch.Prefix,
					Text = record.Name,
					Url = NavigatorExt.AddQueryValueToUri(record.Url, TfConstants.ActiveSaveQueryName, record.Id.ToString()),
					Selected = record.Id == TfAppState.Value.Route.ActiveSaveId
				};
				menuItems.Add(viewMenu);
			}
		}
		_menu = menuItems;
	}
	private void _assignMenuItemActions(TucMenuItem item)
	{
		item.OnClick = async () => await _onMenuItemClick(item);
		item.OnExpand = async () => await _onMenuItemExpand(item);
	}

	private async Task _onMenuItemExpand(TucMenuItem item)
	{
		if (item.Expanded)
		{
			item.Expanded = false;
			await _removeExpandedNodesFromStorage(item.Id);
		}
		else
		{
			item.Expanded = true;
			await _addExpandedNodesToStorage(item.Id);
		}

	}

	private async Task _onMenuItemClick(TucMenuItem item)
	{
		if (item.Data is not null && item.Data is TucSpaceNode)
		{
			var node = (TucSpaceNode)item.Data;
			if (node.Type == TfSpacePageType.Folder)
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

