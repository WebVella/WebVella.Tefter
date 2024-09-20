namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminUserDetailsActions.TfAdminUserDetailsActions", "WebVella.Tefter")]
public partial class TfAdminUserDetailsActions : TfBaseComponent
{
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }


	private async Task _editUser()
	{
		var dialog = await DialogService.ShowDialogAsync<TfUserManageDialog>(
		TfAppState.Value.AdminManagedUser,
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
			ToastService.ShowSuccess(LOC("User successfully updated!"));
			var state = TfAppState.Value with { AdminManagedUser = user};
			Dispatcher.Dispatch(new SetAppStateAction(component: this, state: state));
		}
	}
}