using WebVella.Tefter.Web.Components.UserManageDialog;

namespace WebVella.Tefter.Web.Components.AdminUserNavigation;
public partial class TfAdminUserNavigation : TfBaseComponent, IAsyncDisposable
{
	[Inject] private UserAdminUseCase UC { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.OnInitializedAsync(
		initForm: false,
		initMenu: true
		);
		_setMenuItemActions();
		UC.MenuLoading = false;
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
		ActionSubscriber.SubscribeToAction<UserAdminChangedAction>(this, On_UserDetailsChangedAction);
	}

	private void _setMenuItemActions()
	{
		foreach (var item in UC.MenuItems)
		{
			item.OnClick = () => OnTreeMenuClick(item);
		}
	}

	private async Task loadMoreClick()
	{
		UC.LoadMoreLoading = true;
		await InvokeAsync(StateHasChanged);
		UC.MenuPage++;
		await UC.SetUserMenuAsync();
		_setMenuItemActions();
		UC.LoadMoreLoading = false;
		await InvokeAsync(StateHasChanged);
	}

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfUserManageDialog>(
		new TucUser(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var user = (TucUser)result.Data;
			ToastService.ShowSuccess(LOC("User successfully created!"));
			Navigator.NavigateTo(String.Format(TfConstants.AdminUserDetailsPageUrl, user.Id));
		}
	}

	private void OnTreeMenuSelect(TucMenuItem item, bool selected)
	{
		item.Active = selected;
		if (item.Active && item.Data is not null)
		{
			var user = (TucUser)item.Data;
			ConsoleExt.WriteLine($"OnTreeMenuSelect {user.Id} {selected}");
			Navigator.NavigateTo(item.Url);
		}
	}

	private void OnTreeMenuClick(TucMenuItem item)
	{
		if (item.Data is null) return;
		item.Active = true;
		var user = (TucUser)item.Data;
		Navigator.NavigateTo(item.Url);
	}

	private async Task onSearch(string search)
	{
		if (search == UC.MenuSearch) return;

		UC.MenuLoading = true;
		await InvokeAsync(StateHasChanged);

		UC.MenuSearch = search;
		await UC.OnSearchChanged();
		_setMenuItemActions();

		UC.MenuLoading = false;
		await InvokeAsync(StateHasChanged);
	}

	private void On_UserDetailsChangedAction(UserAdminChangedAction action)
	{
		UC.OnUserDetailsChanged(action.User);
		StateHasChanged();
	}
}