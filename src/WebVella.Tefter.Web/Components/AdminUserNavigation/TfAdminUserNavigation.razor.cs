namespace WebVella.Tefter.Web.Components.AdminUserNavigation;
public partial class TfAdminUserNavigation : TfBaseComponent
{
	[Inject] protected IState<UserState> UserState { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private bool _menuLoading = true;
	private List<MenuItem> _menuItems = new();
	private List<MenuItem> _visibleMenuItems = new();

	private string search = null;
	private bool hasMore = true;
	private int page = 1;
	private int pageSize = 30;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			//SessionState.StateChanged -= SessionState_StateChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			GenerateSpaceDataMenu();
			//SessionState.StateChanged += SessionState_StateChanged;
			_menuLoading = false;
			StateHasChanged();
		}
	}

	//private void SessionState_StateChanged(object sender, EventArgs e)
	//{
	//	InvokeAsync(async () =>
	//	{
	//		if (SessionState.Value.DataHashId == _renderedDataHashId
	//			&& SessionState.Value.Space?.Id == _renderedSpaceId) return;

	//		_menuLoading = true;
	//		await InvokeAsync(StateHasChanged);
	//		await Task.Delay(1);
	//		GenerateSpaceDataMenu();
	//		_menuLoading = false;
	//		await InvokeAsync(StateHasChanged);
	//	});

	//}

	private async void GenerateSpaceDataMenu(string search = null)
	{
		search = search?.Trim().ToLowerInvariant();
		_menuItems.Clear();
		var userResult = await IdentityManager.GetUsersAsync();
		if(userResult.IsFailed) return;
		
		var users = userResult.Value;


		foreach (var item in users)
		{
			var menu = new MenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(item.Id),
				Icon = new Icons.Regular.Size20.Person(),
				Level = 0,
				Match = NavLinkMatch.Prefix,
				Title = String.Join(" ", new List<string>{ item.FirstName, item.LastName}),
				Url = $"/admin/users/{item.Id}",
				Expanded = false,
				Active = false

			};
			SetMenuItemActions(menu);
			_menuItems.Add(menu);

		}

		var batch = _menuItems.Skip(RenderUtils.CalcSkip(pageSize, page)).Take(pageSize).ToList();
		if (batch.Count < pageSize) hasMore = false;
		_visibleMenuItems = batch;
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

		//var spaces = await tfSrv.GetSpacesForUserAsync(UserState.Value.User.Id);
		//Navigator.NavigateTo($"/space/{spaces[0].Id}/data/{spaces[0].Data[0].Id}");
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

	private async Task onSearch(string value)
	{
		search = value;
		GenerateSpaceDataMenu(search);
	}
}