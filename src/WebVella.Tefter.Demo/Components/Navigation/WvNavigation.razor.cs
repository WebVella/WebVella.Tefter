namespace WebVella.Tefter.Demo.Components;
public partial class WvNavigation : WvBaseComponent
{
	private Space _space = null;
	private List<MenuItem> _menuItems = new();

	private bool _selectorMenuVisible = false;
	private bool _settingsMenuVisible = false;
	private WvNavigationSelector _selectorEl;
	private string _sidebarCollapseCss = "";

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			WvState.ActiveSpaceDataChanged -= OnSpaceDataChanged;
		WvState.UISettingsChanged -= OnUISettingsChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if(firstRender){
			var meta = WvState.GetActiveSpaceMeta();
			_space = meta.Space;
			GenerateSpaceDataMenu(_space);
			_sidebarCollapseCss = WvState.GetUiSettings().SidebarExpanded ? "expanded" : "collapsed";
			StateHasChanged();

			WvState.ActiveSpaceDataChanged += OnSpaceDataChanged;
			WvState.UISettingsChanged += OnUISettingsChanged;
		}
	}

	protected void OnSpaceDataChanged(object sender, StateActiveSpaceDataChangedEventArgs args)
	{
		base.InvokeAsync(async () =>
		{
			_space = args.Space;
			GenerateSpaceDataMenu(_space);
			await InvokeAsync(StateHasChanged);
		});
	}

	protected void OnUISettingsChanged(object sender, StateUISettingsChangedEventArgs args)
	{
		base.InvokeAsync(async () =>
		{
			_sidebarCollapseCss = WvState.GetUiSettings().SidebarExpanded ? "expanded" : "collapsed";
			await InvokeAsync(StateHasChanged);
		});

	}

	private void GenerateSpaceDataMenu(Space space)
	{
		_menuItems.Clear();
		if (_space is not null)
		{
			foreach (var item in _space.DataItems)
			{
				_menuItems.Add(new MenuItem
				{
					Id = "wv-" + item.Id,
					Icon = new Icons.Regular.Size20.Database(),
					Match = NavLinkMatch.Prefix,
					Title = item.Name,
					Url = $"/space/{_space.Id}/data/{item.Id}"
				});
			}
		}
	}

	private async Task onAddClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for space creation");

		await Task.Delay(500);
		var spaces = WvState.GetSpaces();

		Navigator.NavigateTo($"/space/{spaces[0].Id}/data/{spaces[0].DataItems[0].Id}");
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
		Navigator.NavigateTo($"/space/{space.Id}/data/{space.DataItems[0].Id}");
	}
}