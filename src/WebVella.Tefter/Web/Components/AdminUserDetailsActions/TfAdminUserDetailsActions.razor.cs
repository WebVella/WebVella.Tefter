using WebVella.Tefter.Web.Components.UserManageDialog;

namespace WebVella.Tefter.Web.Components.AdminUserDetailsActions;
public partial class TfAdminUserDetailsActions : TfBaseComponent
{
	[Inject] protected IState<UserDetailsState> UserDetailsState { get; set; }

	private string menu = "details";
	protected override void OnInitialized()
	{
		base.OnInitialized();
		var urlData = Navigator.GetUrlData();
		if (urlData.SegmentsByIndexDict.ContainsKey(3))
			menu = urlData.SegmentsByIndexDict[3];
	}

	private async Task _editUser(){
		var dialog = await DialogService.ShowDialogAsync<TfUserManageDialog>(UserDetailsState.Value.User, 
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
			ToastService.ShowSuccess(LOC("User successfully updated!"));
			Dispatcher.Dispatch(new SetUserDetailsAction(user));
		}
	}
}