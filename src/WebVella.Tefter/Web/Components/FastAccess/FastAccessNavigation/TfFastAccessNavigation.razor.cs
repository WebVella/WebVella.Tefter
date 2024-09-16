namespace WebVella.Tefter.Web.Components;
public partial class TfFastAccessNavigation : TfBaseComponent
{
	[Inject] protected IState<TfState> TfState { get; set; }

	private bool _menuLoading = false;
	private string _renderedDataHashId = string.Empty;
	private Guid? _renderedSpaceId = null;


	private List<MenuItem> _menuItems = new();
	private List<MenuItem> _visibleMenuItems = new();

	private bool _selectorMenuVisible = false;
	private bool _settingsMenuVisible = false;

	private bool hasMore = true;
	private int page = 1;
	private int pageSize = 30;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		GenerateSpaceDataMenu();
	}

	private void SessionState_StateChanged(object sender, EventArgs e)
	{
		InvokeAsync(async () =>
		{
			_menuLoading = true;
			await InvokeAsync(StateHasChanged);
			GenerateSpaceDataMenu();
			_menuLoading = false;
			await InvokeAsync(StateHasChanged);
		});

	}

	private void GenerateSpaceDataMenu()
	{
		_menuItems.Clear();
		var menu = new MenuItem
		{
			Id = "bookmarks",
			Icon = TfConstants.BookmarkOFFIcon,
			Level = 0,
			Match = NavLinkMatch.Prefix,
			Title = "Bookmarks",
			Nodes = new List<MenuItem>(),
			Expanded = true,
		};

		SetMenuItemActions(menu);
		_menuItems.Add(menu);

		var menu2 = new MenuItem
		{
			Id = "saved",
			Icon = TfConstants.SaveIcon,
			Level = 0,
			Match = NavLinkMatch.Prefix,
			Title = "Saved Views",
			Nodes = new List<MenuItem>(),
		};
		SetMenuItemActions(menu2);
		_menuItems.Add(menu2);

		_visibleMenuItems = _menuItems;
		//_renderedDataHashId = SessionState.Value.DataHashId;
		//_renderedSpaceId = SessionState.Value.Space?.Id;
	}

	private void SetMenuItemActions(MenuItem item)
	{
		item.OnSelect = (selected) => OnTreeMenuSelect(item, selected);
	}

	private void OnTreeMenuSelect(MenuItem item, bool selected)
	{
		if (item.Level == 0)
		{
			item.Expanded = !item.Expanded;
			item.Active = false;
			return;
		}
		item.Active = selected;
		if (item.Active)
		{
			if (item.SpaceId is null || item.SpaceDataId is null || item.SpaceViewId is null) return;
			//Dispatcher.Dispatch(new GetSessionAction(
			//	userId: TfState.Value.User.Id,
			//	spaceId: item.SpaceId.Value,
			//	spaceDataId: item.SpaceDataId.Value,
			//	spaceViewId: item.SpaceViewId.Value));
		}
	}

}