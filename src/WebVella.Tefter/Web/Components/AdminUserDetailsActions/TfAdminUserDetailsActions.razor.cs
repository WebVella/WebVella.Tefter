using WebVella.Tefter.Web.Components.UserManageDialog;

namespace WebVella.Tefter.Web.Components.AdminUserDetailsActions;
public partial class TfAdminUserDetailsActions : TfBaseComponent
{
	[Inject] protected IState<UserAdminState> UserAdminState { get; set; }

	private string menu = "details";

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if(disposing) Navigator.LocationChanged -= Navigator_LocationChanged;
		return base.DisposeAsyncCore(disposing);
	}
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_setMenu(null);
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		_setMenu(e.Location);
		StateHasChanged();
	}

	private void _setMenu(string url)
	{
		var urlData = Navigator.GetUrlData(url);
		if (urlData.SegmentsByIndexDict.ContainsKey(3))
			menu = urlData.SegmentsByIndexDict[3];
		else
			menu = "details";
	}

	private async Task _editUser()
	{
		var dialog = await DialogService.ShowDialogAsync<TfUserManageDialog>(
		UserAdminState.Value.User,
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
			Dispatcher.Dispatch(new SetUserAdminAction(false, user));
		}
	}
}