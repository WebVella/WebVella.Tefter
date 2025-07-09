namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProvidersPageContent :TfBaseComponent
{
	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderManageDialog>(
		new TfDataProvider(), 
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
			var provider = (TfDataProvider)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminDataProviderDetailsPageUrl, provider.Id));
		}
	}
}