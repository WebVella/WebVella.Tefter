namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	[Parameter] public EventCallback<List<TucSort>> OnSort { get; set; }
	private async Task OnSortClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewSortsDialog>(
						true,
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
			await OnSort.InvokeAsync((List<TucSort>)result.Data);
		}
	}

}