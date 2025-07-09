namespace WebVella.Tefter.UI.Components;
public partial class TucAdminSharedColumnsPageContent :TfBaseComponent
{
	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSharedColumnManageDialog>(
		new TfSharedColumn(), 
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
			var item = (TfSharedColumn)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminSharedColumnDetailsPageUrl, item.Id));
		}
	}
}