namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataIdentitiesPageContent : TfBaseComponent
{
	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataIdentityManageDialog>(
		new TfDataIdentity(),
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
			var item = (TfDataIdentity)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminDataIdentityDetailsPageUrl, item.DataIdentity));
		}
	}
}