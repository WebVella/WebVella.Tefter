namespace WebVella.Tefter.Demo.Components;
public partial class WvNavigation : WvBaseComponent,IDisposable
{
	private Space _space = null;
		private List<MenuItem> _menuItems = new();
	
	private bool _selectorMenuVisible = false;
	private bool _settingsMenuVisible = false;
	private WvNavigationSelector _selectorEl;
	public void Dispose()
	{
		WvState.ActiveSpaceDataChanged -= OnSpaceDataChanged;
	}

	protected override void OnInitialized()
	{
		WvState.ActiveSpaceDataChanged += OnSpaceDataChanged;
		//space = spaces[0];

	}

	protected void OnSpaceDataChanged(object sender, StateActiveSpaceDataChangedEventArgs args)
	{
		_space = args.Space;
		GenerateSpaceItemMenu(_space);
		StateHasChanged();
	}

	private void GenerateSpaceItemMenu(Space space){
		_menuItems.Clear();
		if(_space is not null){
			foreach (var item in _space.Items)
			{
				_menuItems.Add(new MenuItem
				{
					Id = item.Id,
					Icon = item.Icon,
					Match = NavLinkMatch.Prefix,
					Title = item.Name,
					Url = $"/space/{_space.Id}/item/{item.Id}"
				});
			}
		}
	}

	private async Task onAddClick(){
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for space creation");

		await Task.Delay(500);
		var spaces = WvState.GetSpaces();
		
		Navigator.NavigateTo($"/space/{spaces[0].Id}/item/{spaces[0].Items[0].Id}");
	}

	private void onRemoveClick(){
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for removing space");
	}
	private void onRenameClick(){
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for renaming");
	}

	private async Task onSpaceSelectorClick(){
		await _selectorEl.ToggleSelector();
	}

	private async Task SpaceChanged(Space space){
		Navigator.NavigateTo($"/space/{space.Id}/item/{space.Items[0].Id}");
	}
}