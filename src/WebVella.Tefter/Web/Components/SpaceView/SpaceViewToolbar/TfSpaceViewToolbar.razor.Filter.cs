namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	[Parameter] public EventCallback<List<TucFilterBase>> OnFilter { get; set; }
	private async Task OnFilterClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewFiltersDialog>(
						true,
						new DialogParameters()
						{
							PreventDismissOnOverlayClick = true,
							PreventScroll = true,
							Width = TfConstants.DialogWidthLarge
						});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			await OnFilter.InvokeAsync((List<TucFilterBase>)result.Data);
		}
	}

}