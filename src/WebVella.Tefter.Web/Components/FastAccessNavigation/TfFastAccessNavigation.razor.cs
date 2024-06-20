namespace WebVella.Tefter.Web.Components;
public partial class TfFastAccessNavigation : TfBaseComponent
{
	[Inject] protected IState<UserState> UserState { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private bool _menuLoading = true;
	private string _renderedDataHashId = string.Empty;
	private Guid? _renderedSpaceId = null;


	private List<MenuItem> _menuItems = new();
	private List<MenuItem> _visibleMenuItems = new();

	private bool _selectorMenuVisible = false;
	private bool _settingsMenuVisible = false;
	private TfSpaceSelector _selectorEl;

	private bool hasMore = true;
	private int page = 1;
	private int pageSize = 30;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			SessionState.StateChanged -= SessionState_StateChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			GenerateSpaceDataMenu();
			SessionState.StateChanged += SessionState_StateChanged;
			_menuLoading = false;
			StateHasChanged();
		}
	}

	private void SessionState_StateChanged(object sender, EventArgs e)
	{
		InvokeAsync(async () =>
		{
			if (SessionState.Value.DataHashId == _renderedDataHashId
				&& SessionState.Value.Space?.Id == _renderedSpaceId) return;

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
			Icon = new Icons.Regular.Size20.Star(),
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
			Icon = new Icons.Regular.Size20.Save(),
			Level = 0,
			Match = NavLinkMatch.Prefix,
			Title = "Saved Views",
			Nodes = new List<MenuItem>(),
		};
		SetMenuItemActions(menu2);
		_menuItems.Add(menu2);

		_visibleMenuItems = _menuItems;
		_renderedDataHashId = SessionState.Value.DataHashId;
		_renderedSpaceId = SessionState.Value.Space?.Id;
	}

	private async Task loadMoreClick()
	{
		var batch = _menuItems.Skip(RenderUtils.CalcSkip(pageSize, page + 1)).Take(pageSize).ToList();
		if (batch.Count < pageSize) hasMore = false;
		_visibleMenuItems.AddRange(batch);
		page++;
		await InvokeAsync(StateHasChanged);
	}

	private async Task onAddClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for space creation");

		var spaces = await TfSrv.GetSpacesForUserAsync(UserState.Value.User.Id);
		Navigator.NavigateTo($"/space/{spaces[0].Id}/data/{spaces[0].Data[0].Id}");
	}
	private void onDetailsClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for details about the space");
	}
	private void onRemoveClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for removing space");
	}
	private void onRenameClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for renaming");
	}

	private async Task onSpaceSelectorClick()
	{
		await _selectorEl.ToggleSelector();
	}

	private async Task SpaceChanged(Space space)
	{
		Navigator.NavigateTo($"/space/{space.Id}/data/{space.Data[0].Id}");
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
			Dispatcher.Dispatch(new GetSessionAction(
				userId: UserState.Value.User.Id,
				spaceId: item.SpaceId.Value,
				spaceDataId: item.SpaceDataId.Value,
				spaceViewId: item.SpaceViewId.Value));
		}
	}

}