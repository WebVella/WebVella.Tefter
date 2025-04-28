namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminRoleDetailsActions.TfAdminRoleDetailsActions", "WebVella.Tefter")]
public partial class TfAdminRoleDetailsActions : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private async Task _editRole()
	{
		var dialog = await DialogService.ShowDialogAsync<TfRoleManageDialog>(
		TfAppState.Value.AdminManagedRole,
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
			var role = (TucRole)result.Data;
			ToastService.ShowSuccess(LOC("Role successfully updated!"));
			var state = TfAppState.Value with { AdminManagedRole = role };
			var roleIndex = TfAppState.Value.UserRoles.FindIndex(x => x.Id == role.Id);
			if (roleIndex > 0)
			{
				var roles = state.UserRoles.ToList();
				roles[roleIndex] = role;
				state = state with { UserRoles = roles };
			}
			Dispatcher.Dispatch(new SetAppStateAction(component: this, state: state));
		}
	}
}