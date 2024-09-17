namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminUserNavigation.TfAdminUserNavigation", "WebVella.Tefter")]
public partial class TfAdminUserNavigation : TfBaseComponent, IAsyncDisposable
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private bool _endIsReached = false;
	private bool _isLoading = false;
	private string _search = null;
	private int _stringLimit = 30;
	private int _pageSize = TfConstants.PageSize;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (TfAppState.Value.AdminUsers.Count < _pageSize)
			_endIsReached = true;
	}
	private async Task loadMoreClick(bool resetList = false)
	{
		var page = TfAppState.Value.AdminUsersPage + 1;
		if (resetList)
		{
			page = 1;
			_endIsReached = false;
		}
		_isLoading = true;
		await InvokeAsync(StateHasChanged);

		var users = await UC.GetUsersAsync(
			search: _search,
			page: page,
			pageSize: _pageSize
		);
		var aggrUsers = TfAppState.Value.AdminUsers;

		if (!resetList)
		{
			aggrUsers.AddRange(users);
		}
		else
		{
			aggrUsers = users;
		}


		Dispatcher.Dispatch(new SetAdminUsersStateAction(
			component: this,
			adminUsers: aggrUsers,
			adminUsersPage: page
		));

		_isLoading = false;
		if (users.Count < _pageSize) _endIsReached = true;
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

	private async Task onSearch(string search)
	{
		_search = search;
		await loadMoreClick(true);
	}

}