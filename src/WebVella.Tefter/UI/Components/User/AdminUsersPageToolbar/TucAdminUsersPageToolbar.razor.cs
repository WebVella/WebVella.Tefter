namespace WebVella.Tefter.UI.Components;
public partial class TucAdminUsersPageToolbar : TfBaseComponent
{
	private async Task addUser()
	{
		var dialog = await DialogService.ShowDialogAsync<TucUserManageDialog>(
		new TfUser(),
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
			var user = (TfUser)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminUserDetailsPageUrl, user.Id));
		}

	}
}