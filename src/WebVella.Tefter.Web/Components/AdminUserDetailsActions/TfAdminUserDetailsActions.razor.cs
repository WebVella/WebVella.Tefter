﻿using WebVella.Tefter.Web.Components.UserManageDialog;

namespace WebVella.Tefter.Web.Components.AdminUserDetailsActions;
public partial class TfAdminUserDetailsActions : TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private async Task _editUser(){
		var dialog = await DialogService.ShowDialogAsync<TfUserManageDialog>(SessionState.Value.CurrentAdminUser, 
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var user = (User)result.Data;
			ToastService.ShowSuccess("User successfully updated!");
			Dispatcher.Dispatch(new SetCurrentAdminUser(user));
		}
	}
}