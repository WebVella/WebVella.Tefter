namespace WebVella.Tefter.UI.Components;

public partial class TucHomeDashboard : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;

	private List<TfBookmark> _allBookmarks = new();
	private List<TfBookmark> _bookmarks = new();
	private int _page = 1;
	private int _pageSize = 32;
	private string? _search = null;
	private TucMyBookmarksOrder _order = TucMyBookmarksOrder.Newest;
	private FluentSearch _refSearch = null!;

	private readonly List<TfMenuItem> _nav = new();
	private readonly List<TfMenuItem> _tagNav = new();

	public void Dispose()
	{
		TfEventProvider.BookmarkUpdatedEvent -= On_BookmarkChangedEvent;
		TfEventProvider.BookmarkDeletedEvent -= On_BookmarkChangedEvent;
		Navigator.LocationChanged -= On_LocationChanged;
		TfEventProvider.Dispose();
	}
	protected override void OnInitialized()
	{
			_allBookmarks = TfAuthLayout.GetState().UserBookmarks.ToList();
		
			//_allBookmarks = TfAuthLayout.GetState().UserSaves.ToList();
		_init();
		TfEventProvider.BookmarkUpdatedEvent += On_BookmarkChangedEvent;
		TfEventProvider.BookmarkDeletedEvent += On_BookmarkChangedEvent;
		Navigator.LocationChanged += On_LocationChanged;
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			_refSearch.FocusAsync();
		}
	}

	private async Task On_BookmarkChangedEvent(TfGlobalEvent args)
	{
		if (args.IsUserApplicable(this))
		{
				_allBookmarks = TfService.GetBookmarksListForUser(TfAuthLayout.GetState().User.Id);
				//_allBookmarks = TfService.GetSavesListForUser(TfAuthLayout.GetState().User.Id);
			_init();
			await InvokeAsync(StateHasChanged);
		}
	}

	private void On_LocationChanged(object? caller, LocationChangedEventArgs args)
	{
		_init();
		StateHasChanged();
	}

	private void _initNav()
	{
		_nav.Clear();
		_tagNav.Clear();
		
		_nav.Add(new TfMenuItem()
		{
			Text = "Bookmarks",
			IconCollapsed = TfConstants.GetIcon("Star"),
			Url = "/",
			Selected = true
		});
		_nav.Add(new TfMenuItem()
		{
			Text = "Saved URLs",
			IconCollapsed = TfConstants.GetIcon("Link"),
			Url = "/"
		});

		_tagNav.Add(new TfMenuItem()
		{
			Text = "Test",
			IconCollapsed = new CustomIcons.HashTag(),
			Url = "/"
		});		
	}

	private List<TfMenuItem> _getActionsMenu(TfBookmark bookmark)
	{
		var menu = new List<TfMenuItem>
		{
			new TfMenuItem()
			{
				Disabled=true,
				Text = $"{bookmark.CreatedOn.ToString(TfConstants.DateHourFormat)}",
				IconCollapsed = TfConstants.GetIcon("Clock")!.WithColor(Color.Neutral),
			},
			new TfMenuItem()
			{
				IsDivider = true
			},
			new TfMenuItem()
			{
				Text = "Edit",
				IconCollapsed = TfConstants.GetIcon("Edit"),
				OnClick = EventCallback.Factory.Create(this, async () => await _onEdit(bookmark))
			},
			new TfMenuItem()
			{
				Text = "Delete",
				IconCollapsed = TfConstants.GetIcon("Delete")!.WithColor(Color.Error),
				OnClick = EventCallback.Factory.Create(this, async () => await _onRemove(bookmark))
			}
		};
		return menu;
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

		var queryDict = new Dictionary<string, object?>
		{
			{ TfConstants.PageQueryName, newPage }
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _goOnPage(int page)
	{
		var navState = TfAuthLayout.GetState().NavigationState;
		if (page < 1 && page != -1) page = 1;
		if (navState.Page == page) return;
		var queryDict = new Dictionary<string, object?>
		{
			{ TfConstants.PageQueryName, page }
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _searchValueChanged(string? search)
	{
		var queryDict = new Dictionary<string, object?>{
			{TfConstants.SearchQueryName,search}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _orderChanged(TucMyBookmarksOrder order)
	{
		var queryDict = new Dictionary<string, object?>{
			{TfConstants.OrderQueryName,((int)order).ToString()}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private void _init()
	{
		var state = TfAuthLayout.GetState().NavigationState;
		_page = state.Page ?? 1;
		_pageSize = state.PageSize ?? 32;
		_search = state.Search;
		_order = Navigator.GetEnumFromQuery<TucMyBookmarksOrder>(TfConstants.OrderQueryName, TucMyBookmarksOrder.Newest)!.Value;
		var search = _search?.ToLowerInvariant();
		_bookmarks = _allBookmarks.ToList();
		if (!String.IsNullOrWhiteSpace(_search))
			_bookmarks = _bookmarks.Where(x =>
					x.Name.ToLowerInvariant().Contains(search!)
					|| x.Description.ToLowerInvariant().Contains(search!)
					|| x.Space!.Name!.ToLowerInvariant().Contains(search!)
					|| x.SpacePage!.Name.ToLowerInvariant().Contains(search!)).ToList();

		switch (_order)
		{
			case TucMyBookmarksOrder.Oldest:
				_bookmarks = _bookmarks.OrderBy(x => x.CreatedOn).ToList();
				break;
			case TucMyBookmarksOrder.Alpha:
				_bookmarks = _bookmarks.OrderBy(x => x.Name).ToList();
				break;
			case TucMyBookmarksOrder.AlphaReverse:
				_bookmarks = _bookmarks.OrderByDescending(x => x.Name).ToList();
				break;
			default:
				_bookmarks = _bookmarks.OrderByDescending(x => x.CreatedOn).ToList();
				break;
		}
		_bookmarks = _bookmarks.Skip((_page - 1) * _pageSize).Take(_pageSize).ToList();
		_initNav();
	}



	public enum TucMyBookmarksOrder
	{
		Newest = 0,
		Oldest = 1,
		Alpha = 2,
		AlphaReverse = 3,
	}
}