using WebVella.Tefter.Web.Components.UserManageDialog;

namespace WebVella.Tefter.Web.Components.AdminUserNavigation;
public partial class TfAdminUserNavigation : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected IState<SessionState> SessionState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }

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
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await GenerateSpaceDataMenu();
			ActionSubscriber.SubscribeToAction<UserDetailsChangedAction>(this, On_GetUserDetailsActionResult);
			_menuLoading = false;
			StateHasChanged();
		}
	}

	private async Task GenerateSpaceDataMenu(string search = null)
	{
		search = search?.Trim().ToLowerInvariant();
		_menuItems.Clear();
		var userResult = await IdentityManager.GetUsersAsync();
		if (userResult.IsFailed) return;
		var users = userResult.Value.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();
		foreach (var item in users)
		{
			if (!String.IsNullOrWhiteSpace(search) && !item.FirstName.ToLowerInvariant().Contains(search)
			 && !item.LastName.ToLowerInvariant().Contains(search))
				continue;
			var menu = new MenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(item.Id),
				Data = item,
				Icon = new Icons.Regular.Size20.Person(),
				Level = 0,
				Match = NavLinkMatch.Prefix,
				Title = String.Join(" ", new List<string> { item.FirstName, item.LastName }),
				Url = $"/admin/users/{item.Id}",
				Active = Navigator.GetUrlData().UserId == item.Id,

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
		var dialog = await DialogService.ShowDialogAsync<TfUserManageDialog>(null, new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var user = (User)result.Data;
			ToastService.ShowSuccess("User successfully created!");
			Dispatcher.Dispatch(new SetUserDetailsAction(user));
			Navigator.NavigateTo(String.Format(TfConstants.AdminUserDetailsPageUrl, user.Id));
		}
	}

	private void SetMenuItemActions(MenuItem item)
	{
		item.OnSelect = (selected) => OnTreeMenuSelect(item, selected);
	}

	private void OnTreeMenuSelect(MenuItem item, bool selected)
	{
		item.Active = selected;
		if (item.Active && item.Data is not null)
		{
			var user = (User)item.Data;
			Navigator.NavigateTo(String.Format(TfConstants.AdminUserDetailsPageUrl, user.Id));
		}
	}

	private async Task onSearch(string value)
	{
		search = value;
		await GenerateSpaceDataMenu(search);
		await InvokeAsync(StateHasChanged);
	}

	private void On_GetUserDetailsActionResult(UserDetailsChangedAction action)
	{
		InvokeAsync(async () =>
		{
			await GenerateSpaceDataMenu(search);
			await InvokeAsync(StateHasChanged);
		});
	}
}