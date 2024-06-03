namespace WebVella.Tefter.Demo.Components;
public partial class WvUserNavigation : WvBaseComponent
{
    private bool _visible = false;

    private void _onClick(){ 
        _visible = !_visible;
    }

    private async Task _setTheme(){
        var user = WvState.GetUser();
        var dialog = await DialogService.ShowDialogAsync<WvSetThemeDialog>(user, new DialogParameters()
        {
            PreventDismissOnOverlayClick = true,
            PreventScroll = true,
        });
        var result = await dialog.Result;
        if (!result.Cancelled && result.Data != null)
        {
            user = (User)result.Data;
            await WvState.SetTheme(user.ThemeMode,user.ThemeColor);
        }
    }
    private void _editProfile(){
		ToastService.ShowToast(ToastIntent.Warning, "Will open edit profile modal");
	}
    private void _logout(){
		ToastService.ShowToast(ToastIntent.Warning, "Will logout user");
	}

	private void _alertsClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Navigates to usr alerts");
	}

	private void _adminClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Navigates to Administration panel");
	}


}