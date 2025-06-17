namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataIdentityDetailsActions.TfAdminDataIdentityDetailsActions", "WebVella.Tefter")]
public partial class TfAdminDataIdentityDetailsActions : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private async Task _editIdentity()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataIdentityManageDialog>(
		TfAppState.Value.AdminDataIdentity,
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
			var item = (TucDataIdentity)result.Data;
			ToastService.ShowSuccess(LOC("Role successfully updated!"));
			var state = TfAppState.Value with { AdminDataIdentity = item };
			var itemIndex = TfAppState.Value.AdminDataIdentities.FindIndex(x => x.Name == item.Name);
			if (itemIndex > 0)
			{
				var newList = state.AdminDataIdentities.ToList();
				newList[itemIndex] = item;
				state = state with { AdminDataIdentities = newList };
			}
			Dispatcher.Dispatch(new SetAppStateAction(component: this, state: state));
		}
	}
}