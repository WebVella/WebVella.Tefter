using WebVella.Tefter.UI.Pages;

namespace WebVella.Tefter.UI.Components;

public partial class TucHomeDashboard : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;

	private List<TfDashboardItem> _dashboard = new();
	private List<TfBookmark> _allBookmarks = new();
	private List<TfTag> _tags = new();
	private int _page = 1;
	private int _pageSize = 32;
	private string? _search = null;
	private string? _tagQuery = null;
	private TucMyBookmarksOrder _order = TucMyBookmarksOrder.Newest;
	private FluentSearch? _refSearch = null;

	private readonly List<TfMenuItem> _nav = new();
	private readonly List<TfMenuItem> _tagNav = new();

	private TucMyBookmarksTab _activeTab = TucMyBookmarksTab.Bookmarks;

	public void Dispose()
	{
		TfEventProvider.BookmarkUpdatedEvent -= On_BookmarkChangedEvent;
		TfEventProvider.BookmarkDeletedEvent -= On_BookmarkChangedEvent;
		Navigator.LocationChanged -= On_LocationChanged;
		TfEventProvider.Dispose();
	}

	protected override void OnInitialized()
	{
		_initBookmarks();
		_init();
		TfEventProvider.BookmarkUpdatedEvent += On_BookmarkChangedEvent;
		TfEventProvider.BookmarkDeletedEvent += On_BookmarkChangedEvent;
		Navigator.LocationChanged += On_LocationChanged;
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			if (_refSearch is not null)
				_refSearch.FocusAsync();
		}
	}

	private async Task On_BookmarkChangedEvent(TfGlobalEvent args)
	{
		if (args.IsUserApplicable(this))
		{
			_initBookmarks();
			_init();
			await InvokeAsync(StateHasChanged);
		}
	}

	private void On_LocationChanged(object? caller, LocationChangedEventArgs args)
	{
		_init();
		StateHasChanged();
	}

	private void _initBookmarks()
	{
		_allBookmarks = TfService.GetBookmarksForUser(TfAuthLayout.GetState().User.Id);
		_tags.Clear();
		var addedHs = new HashSet<Guid>();
		foreach (var bookmark in _allBookmarks)
		{
			foreach (var tag in bookmark.Tags)
			{
				if (!addedHs.Add(tag.Id)) continue;
				_tags.Add(tag);
			}
		}

		_tags = _tags.OrderBy(x => x.Label).ToList();
	}

	private void _init()
	{
		var state = TfAuthLayout.GetState().NavigationState;
		_page = state.Page ?? 1;
		_pageSize = state.PageSize ?? 32;
		_search = state.Search;
		_order =
			Navigator.GetEnumFromQuery<TucMyBookmarksOrder>(TfConstants.OrderQueryName, TucMyBookmarksOrder.Newest)!
				.Value;
		var tagQuery = Navigator.GetStringFromQuery(TfConstants.TagQueryName);
		var search = _search?.ToLowerInvariant();
		_activeTab = Navigator.GetEnumFromQuery<TucMyBookmarksTab>(TfConstants.TabQueryName, TucMyBookmarksTab.All)!
			.Value;
		_dashboard.Clear();

		//bookmarks
		if (_activeTab == TucMyBookmarksTab.All
			|| _activeTab == TucMyBookmarksTab.Bookmarks)
		{
			var bookmarks = _allBookmarks.Where(x => x.Type == TfBookmarkType.URL);
			if (!String.IsNullOrWhiteSpace(_search))
				bookmarks = bookmarks.Where(x =>
					x.Name.ToLowerInvariant().Contains(search!)
					|| (x.Description is null || x.Description.ToLowerInvariant().Contains(search!))
					|| x.Space!.Name!.ToLowerInvariant().Contains(search!)
					|| x.SpacePage!.Name.ToLowerInvariant().Contains(search!));
			if (!String.IsNullOrWhiteSpace(tagQuery))
				bookmarks = bookmarks.Where(x => x.Tags.Any(y => y.Label == tagQuery));

			foreach (var item in bookmarks)
			{
				_dashboard.Add(new TfDashboardItem(
					bookmark: item,
					navigator: Navigator,
					tagLinkTitle: LOC("click to filter by tag"),
					menu: _getItemMenu(item),
					actions: _getItemActions(item)));
			}
		}

		//pinned pages
		if (_activeTab == TucMyBookmarksTab.All
			|| _activeTab == TucMyBookmarksTab.PinnedPages)
		{
			var bookmarks = _allBookmarks.Where(x => x.Type == TfBookmarkType.Page);
			if (!String.IsNullOrWhiteSpace(_search))
				bookmarks = bookmarks.Where(x =>
					x.Name.ToLowerInvariant().Contains(search!)
					|| (x.Description is null || x.Description.ToLowerInvariant().Contains(search!))
					|| x.Space!.Name!.ToLowerInvariant().Contains(search!)
					|| x.SpacePage!.Name.ToLowerInvariant().Contains(search!));
			if (!String.IsNullOrWhiteSpace(tagQuery))
				bookmarks = bookmarks.Where(x => x.Tags.Any(y => y.Label == tagQuery));

			foreach (var item in bookmarks)
			{
				_dashboard.Add(new TfDashboardItem(
					bookmark: item,
					navigator: Navigator,
					tagLinkTitle: LOC("click to filter by tag"),
					menu: _getItemMenu(item),
					actions: _getItemActions(item)
					));
			}
		}
		//pinned data
		if (_activeTab == TucMyBookmarksTab.All
			|| _activeTab == TucMyBookmarksTab.PinnedData)
		{
			var bookmarks = _allBookmarks.Where(x => x.Type == TfBookmarkType.DataProviderRows);
			if (!String.IsNullOrWhiteSpace(_search))
				bookmarks = bookmarks.Where(x =>
					x.Name.ToLowerInvariant().Contains(search!)
					|| (x.Description is null || x.Description.ToLowerInvariant().Contains(search!))
					|| x.Space!.Name!.ToLowerInvariant().Contains(search!)
					|| x.SpacePage!.Name.ToLowerInvariant().Contains(search!));
			if (!String.IsNullOrWhiteSpace(tagQuery))
				bookmarks = bookmarks.Where(x => x.Tags.Any(y => y.Label == tagQuery));

			foreach (var item in bookmarks)
			{
				_dashboard.Add(new TfDashboardItem(
					bookmark: item,
					navigator: Navigator,
					tagLinkTitle: LOC("click to filter by tag"),
					menu: _getItemMenu(item),
					actions: _getItemActions(item)
					));
			}
		}

		switch (_order)
		{
			case TucMyBookmarksOrder.Oldest:
				_dashboard = _dashboard.OrderBy(x => x.CreatedOn).ToList();
				break;
			case TucMyBookmarksOrder.Alpha:
				_dashboard = _dashboard.OrderBy(x => x.Title).ToList();
				break;
			case TucMyBookmarksOrder.AlphaReverse:
				_dashboard = _dashboard.OrderByDescending(x => x.Title).ToList();
				break;
			default:
				_dashboard = _dashboard.OrderByDescending(x => x.CreatedOn).ToList();
				break;
		}

		_dashboard = _dashboard.Skip((_page - 1) * _pageSize).Take(_pageSize).ToList();

		_initNav();
	}

	private void _initNav()
	{
		_nav.Clear();
		_tagNav.Clear();
		_tagQuery = Navigator.GetStringFromQuery(TfConstants.TagQueryName);
		_activeTab = Navigator.GetEnumFromQuery<TucMyBookmarksTab>(TfConstants.TabQueryName, TucMyBookmarksTab.All)!
			.Value;
		//Global menu
		_nav.Add(new TfMenuItem()
		{
			Text = "All items",
			IconCollapsed = TfConstants.GetIcon("ListBar"),
			Url = Navigator.GetLocalAndQueryUrl()
				.ApplyChangeToUrlQuery(TfConstants.TabQueryName, null),
			Selected = _activeTab == TucMyBookmarksTab.All
		});
		_nav.Add(new TfMenuItem()
		{
			Text = "Bookmarks",
			IconCollapsed = TfConstants.GetIcon("Star"),
			Url = Navigator.GetLocalAndQueryUrl()
				.ApplyChangeToUrlQuery(TfConstants.TabQueryName, TucMyBookmarksTab.Bookmarks),
			Selected = _activeTab == TucMyBookmarksTab.Bookmarks
		});

		_nav.Add(new TfMenuItem()
		{
			Text = "Pinned Pages",
			IconCollapsed = TfConstants.GetIcon("Document"),
			Url = Navigator.GetLocalAndQueryUrl()
				.ApplyChangeToUrlQuery(TfConstants.TabQueryName, TucMyBookmarksTab.PinnedPages),
			Selected = _activeTab == TucMyBookmarksTab.PinnedPages
		});
		_nav.Add(new TfMenuItem()
		{
			Text = "Pinned Records",
			IconCollapsed = TfConstants.GetIcon("Database"),
			Url = Navigator.GetLocalAndQueryUrl()
				.ApplyChangeToUrlQuery(TfConstants.TabQueryName, TucMyBookmarksTab.PinnedData),
			Selected = _activeTab == TucMyBookmarksTab.PinnedData
		});

		//tag menu
		foreach (var tag in _tags)
		{
			_tagNav.Add(new TfMenuItem()
			{
				Text = tag.Label,
				IconCollapsed = new CustomIcons.HashTag(),
				Url = Navigator.GetLocalAndQueryUrl().ApplyChangeToUrlQuery(TfConstants.TagQueryName,
					tag.Label == _tagQuery ? null : tag.Label),
				Selected = tag.Label == _tagQuery
			});
		}
	}

	private List<TfMenuItem> _getItemMenu(TfBookmark bookmark)
	{
		var menu = new List<TfMenuItem>
		{
			new TfMenuItem()
			{
				Disabled = true,
				Text = $"{bookmark.CreatedOn.ToString(TfConstants.DateHourFormat)}",
				IconCollapsed = TfConstants.GetIcon("Clock")!.WithColor(Color.Neutral),
			},
			new TfMenuItem() { IsDivider = true },
			new TfMenuItem()
			{
				Text = "Edit Bookmark",
				IconCollapsed = TfConstants.GetIcon("Edit"),
				OnClick = EventCallback.Factory.Create(this, async () => await _onEdit(bookmark))
			},
			new TfMenuItem()
			{
				Text = "Remove Bookmark",
				IconCollapsed = TfConstants.GetIcon("Delete")!.WithColor(Color.Error),
				OnClick = EventCallback.Factory.Create(this, async () => await _onRemove(bookmark))
			}
		};
		return menu;
	}

	private List<TfMenuItem> _getItemActions(TfBookmark bookmark)
	{
		List<TfMenuItem> menu = new();
		switch (bookmark.Type)
		{
			case TfBookmarkType.URL:
				menu.Add(new TfMenuItem() { Text = "browse", Url = bookmark.GetUrl(), });
				break;
			case TfBookmarkType.Page:
				menu.Add(new TfMenuItem() { Text = "browse", Url = bookmark.GetUrl(), });
				break;
			case TfBookmarkType.DataProviderRows:
				menu.Add(new TfMenuItem() { Text = "quick view", OnClick = EventCallback.Factory.Create(this, async () => await _onQuickView(bookmark)) });
				menu.Add(new TfMenuItem() { Text = "browse", Url = bookmark.GetUrl(), });
				break;
		}
		return menu;
	}

	private async Task _onQuickView(TfBookmark bookmark)
	{
		var context = new TucSpacePageQuickViewDialogContext
		{
			SpacePageId = bookmark.SpacePageId,
			DataIdentity = TfConstants.TEFTER_DEFAULT_OBJECT_NAME,
			RelDataIdentity = TfConstants.TEFTER_DEFAULT_OBJECT_NAME,
			RelIdentityValues = new List<string> { bookmark.Id.ToSha1()}
		};
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageQuickViewDialog>(
			context,
			new()
			{
				PreventDismissOnOverlayClick = false,
				PreventScroll = true,
				Width = TfConstants.DialogWidthContentScreen,
				TrapFocus = true
			});
		_ = await dialog.Result;
	}

	private async Task _onEdit(TfBookmark bookmark)
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewBookmarkManageDialog>(
			bookmark,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}

	private Task _onRemove(TfBookmark bookmark)
	{
		TfService.DeleteBookmark(bookmark.Id);
		ToastService.ShowSuccess(LOC("Bookmark removed"));
		return Task.CompletedTask;
	}
	private async Task _goLastPage()
	{
		int newPage = (int)(_allBookmarks.Count / _pageSize) + 1;

		var queryDict = new Dictionary<string, object?> { { TfConstants.PageQueryName, newPage } };
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _goOnPage(int page)
	{
		var navState = TfAuthLayout.GetState().NavigationState;
		if (page < 1 && page != -1) page = 1;
		if (navState.Page == page) return;
		var queryDict = new Dictionary<string, object?> { { TfConstants.PageQueryName, page } };
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _searchValueChanged(string? search)
	{
		var queryDict = new Dictionary<string, object?> { { TfConstants.SearchQueryName, search } };
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _orderChanged(TucMyBookmarksOrder order)
	{
		var queryDict = new Dictionary<string, object?> { { TfConstants.OrderQueryName, ((int)order).ToString() } };
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}


	private void _removeTagQuery()
	{
		Navigator.NavigateTo(Navigator.GetLocalAndQueryUrl().ApplyChangeToUrlQuery(TfConstants.TagQueryName, null));
	}

	public enum TucMyBookmarksTab
	{
		All = 0,
		Bookmarks = 1,
		PinnedPages = 2,
		PinnedData = 3
	}

	public enum TucMyBookmarksOrder
	{
		Newest = 0,
		Oldest = 1,
		Alpha = 2,
		AlphaReverse = 3,
	}
}