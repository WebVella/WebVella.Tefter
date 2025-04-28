namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminUserDetailsActions.TfAdminUserDetailsActions", "WebVella.Tefter")]
public partial class TfAdminUserDetailsActions : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private async Task _editUser()
	{
		var dialog = await DialogService.ShowDialogAsync<TfUserManageDialog>(
		TfAppState.Value.AdminManagedUser,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var user = (TucUser)result.Data;
			ToastService.ShowSuccess(LOC("User successfully updated!"));
			var state = TfAppState.Value with { AdminManagedUser = user};
			var recIndex = TfAppState.Value.AdminUsers.FindIndex(x => x.Id == user.Id);
			if (recIndex > 0)
			{
				var users = state.AdminUsers.ToList();
				users[recIndex] = user;
				state = state with { AdminUsers = users };
			}
			Dispatcher.Dispatch(new SetAppStateAction(component: this, state: state));
		}
	}
}