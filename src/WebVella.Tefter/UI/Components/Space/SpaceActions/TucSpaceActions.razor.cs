namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceActions : TfBaseComponent
{
	private async Task _findSpace()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceFinderDialog>(
			TfAuthLayout.GetState().User,
			new DialogParameters()
			{
				PreventDismissOnOverlayClick = false,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}
}
