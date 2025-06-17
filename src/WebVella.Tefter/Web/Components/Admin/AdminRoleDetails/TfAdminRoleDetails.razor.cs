namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminRoleDetails.TfAdminRoleDetails", "WebVella.Tefter")]
public partial class TfAdminRoleDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	internal List<TucUser> _userOptions
	{
		get
		{
			if (_roleUsers.Count == 0) return TfAppState.Value.AdminUsers;
			return TfAppState.Value.AdminUsers.Where(x => !_roleUsers.Any(u => x.Id == u.Id)).ToList();
		}
	}

	internal List<TucUser> _roleUsers = new();

	private TucUser _selectedUser = null;
	public bool _submitting = false;
	public Guid? _removingUserId = null;

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
		_roleUsers = await UC.GetUsersForRoleAsync(TfAppState.Value.AdminManagedRole.Id);
		ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
	}

	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(async () =>
		{
			_roleUsers = await UC.GetUsersForRoleAsync(TfAppState.Value.AdminManagedRole.Id);
			await InvokeAsync(StateHasChanged);
		});
	}

	private async Task _addUser()
	{
		if (_submitting) return;

		if (_selectedUser is null) return;
		if (_roleUsers.Any(x => x.Id == _selectedUser.Id)) return;

		try
		{
			_submitting = true;
			await UC.AddUserToRoleAsync(TfAppState.Value.AdminManagedRole.Id, _selectedUser.Id);
			_roleUsers.Add(_selectedUser);
			ToastService.ShowSuccess(LOC("User added"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			_selectedUser = null;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _removeUser(TucUser user)
	{
		if (_removingUserId is not null) return;
		if (user is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this user unassigned?")))
			return;
		try
		{
			_removingUserId = user.Id;
			await UC.RemoveUserFromRoleAsync(TfAppState.Value.AdminManagedRole.Id, user.Id);
			_roleUsers = _roleUsers.Where(x => x.Id != user.Id).ToList();
			ToastService.ShowSuccess(LOC("User removed"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_removingUserId = null;
			await InvokeAsync(StateHasChanged);
		}
	}
}