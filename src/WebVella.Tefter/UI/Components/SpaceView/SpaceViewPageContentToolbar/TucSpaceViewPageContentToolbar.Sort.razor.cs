namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar : TfBaseComponent
{
	private async Task OnSortClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewSortsDialog>(
						SpaceView.Id,
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
			await TucSpaceViewPageContent.OnSort((List<TfSortQuery>)result.Data);
		}
	}
}