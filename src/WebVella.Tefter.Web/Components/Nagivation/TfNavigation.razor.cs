namespace WebVella.Tefter.Web.Components;
public partial class TfNavigation : TfBaseComponent
{
	[Inject] protected IState<UserState> UserState { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private List<MenuItem> _menuItems = new();

	private bool _selectorMenuVisible = false;
	private bool _settingsMenuVisible = false;
	private TfSpaceSelector _selectorEl;

	//protected override async ValueTask DisposeAsyncCore(bool disposing)
	//{
	//	if (disposing)
	//	{
	//	}
	//	await base.DisposeAsyncCore(disposing);
	//}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			GenerateSpaceDataMenu();
			StateHasChanged();
		}
	}

	private void GenerateSpaceDataMenu()
	{
		_menuItems.Clear();
		if (SessionState.Value.Space is not null)
		{
			foreach (var item in SessionState.Value.Space.DataItems)
			{
				_menuItems.Add(new MenuItem
				{
					Id = "wv-" + item.Id,
					Icon = new Icons.Regular.Size20.Database(),
					Match = NavLinkMatch.Prefix,
					Title = item.Name,
					Url = $"/space/{SessionState.Value.Space.Id}/data/{item.Id}"
				});
			}
		}
	}

	private async Task onAddClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for space creation");

		var spaces = await tfSrv.GetSpacesForUserAsync(UserState.Value.User.Id);
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