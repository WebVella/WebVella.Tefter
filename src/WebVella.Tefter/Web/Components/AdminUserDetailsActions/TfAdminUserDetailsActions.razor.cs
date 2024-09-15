namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminUserDetailsActions.TfAdminUserDetailsActions","WebVella.Tefter")]
public partial class TfAdminUserDetailsActions : TfBaseComponent
{
	[Inject] private UserAdminUseCase UC { get; set; }
	[Inject] protected IState<TfState> TfState { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing) Navigator.LocationChanged -= Navigator_LocationChanged;
		return base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		InvokeAsync(async () =>
		{
			await UC.InitActionsMenu(e.Location);
			await InvokeAsync(StateHasChanged);
		});
	}

	private async Task _editUser()
	{
		var dialog = await DialogService.ShowDialogAsync<TfUserManageDialog>(
		TfState.Value.CurrentUser,
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
			Dispatcher.Dispatch(new SetUserAdminAction(component:this,userDetails:user));
		}
	}
}