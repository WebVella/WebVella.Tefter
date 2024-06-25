using WebVella.Tefter.Web.Components.SpaceSelector;

namespace WebVella.Tefter.Web.Components.SpaceNavigation;
public partial class TfSpaceNavigation : TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }

	private bool _menuLoading = true;
	private string _renderedDataHashId = string.Empty;
	private Guid? _renderedSpaceId = null;


	private List<MenuItem> _menuItems = new();
	private List<MenuItem> _visibleMenuItems = new();

	private bool _selectorMenuVisible = false;
	private bool _settingsMenuVisible = false;
	private TfSpaceSelector _selectorEl;


	private string search = null;
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
	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
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
			await Task.Delay(1);
			GenerateSpaceDataMenu();
			_menuLoading = false;
			await InvokeAsync(StateHasChanged);
		});

	}

	private void GenerateSpaceDataMenu(string search = null)
	{
		search = search?.Trim().ToLowerInvariant();
		_menuItems.Clear();
		if (SessionState.Value.Space is not null)
		{
			foreach (var item in SessionState.Value.Space.Data)
			{
				var nodes = new List<MenuItem>();
				var hasSelected = false;
				foreach (var view in item.Views)
				{
					if (!String.IsNullOrWhiteSpace(search) && !view.Name.ToLowerInvariant().Contains(search))
						continue;
					var viewMenu = new MenuItem
					{
						Id = RenderUtils.ConvertGuidToHtmlElementId(view.Id),
						Icon = new Icons.Regular.Size20.Grid(),
						Match = NavLinkMatch.Prefix,
						Level = 1,
						Title = view.Name,
						Url = $"/space/{SessionState.Value.Space.Id}/data/{item.Id}/view/{view.Id}",
						SpaceId = SessionState.Value.Space.Id,
						SpaceDataId = item.Id,
						SpaceViewId = view.Id,
						Active = view.Id == SessionState.Value.SpaceView?.Id,
						Expanded = false
					};
					if (viewMenu.Active)
					{
						hasSelected = true;
					}
					SetMenuItemActions(viewMenu);
					if (String.IsNullOrWhiteSpace(search))
						nodes.Add(viewMenu);
					else
						_menuItems.Add(viewMenu);
				}
				var dataMenu = new MenuItem
				{
					Id = RenderUtils.ConvertGuidToHtmlElementId(item.Id),
					Icon = new Icons.Regular.Size20.Database(),
					Level = 0,
					Match = NavLinkMatch.Prefix,
					Title = item.Name,
					Url = $"/space/{SessionState.Value.Space.Id}/data/{item.Id}",
					Nodes = nodes,
					Expanded = hasSelected,
					Active = false

				};
				SetMenuItemActions(dataMenu);
				if (String.IsNullOrWhiteSpace(search))
					_menuItems.Add(dataMenu);

			}
		}
		var batch = _menuItems.Skip(RenderUtils.CalcSkip(pageSize, page)).Take(pageSize).ToList();
		if (batch.Count < pageSize) hasMore = false;
		_visibleMenuItems = batch;
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

		var spaces = await TfSrv.GetSpacesForUserAsync(SessionState.Value.UserId);
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
				userId: SessionState.Value.UserId,
				spaceId: item.SpaceId.Value,
				spaceDataId: item.SpaceDataId.Value,
				spaceViewId: item.SpaceViewId.Value));
		}
	}

	private async Task onSearch(string value)
	{
		search = value;
		GenerateSpaceDataMenu(search);
		await InvokeAsync(StateHasChanged);
	}
}