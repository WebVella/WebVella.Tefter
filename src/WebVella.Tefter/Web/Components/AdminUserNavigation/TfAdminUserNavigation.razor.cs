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
		await UC.Init(this.GetType());
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
		ActionSubscriber.SubscribeToAction<UserAdminChangedAction>(this, On_UserDetailsChangedAction);
	}

	private async Task loadMoreClick()
	{
		UC.LoadMoreLoading = true;
		await InvokeAsync(StateHasChanged);
		UC.MenuPage++;
		await UC.InitMenuAsync();
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

	private void OnTreeMenuClick(TucMenuItem item)
	{
		if (item.Data is null) return;
		item.Active = true;
		Navigator.NavigateTo(item.Url);
	}

	private async Task onSearch(string search)
	{
		if (search == UC.MenuSearch) return;

		UC.MenuLoading = true;
		await InvokeAsync(StateHasChanged);

		UC.MenuSearch = search;
		await UC.NavigationOnSearchChanged();

		UC.MenuLoading = false;
		await InvokeAsync(StateHasChanged);
	}

	private void On_UserDetailsChangedAction(UserAdminChangedAction action)
	{
		UC.NavigationOnStateChanged(action.User);
		StateHasChanged();
	}
}