namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	[Parameter] public EventCallback<List<TucFilterBase>> OnFilter { get; set; }
	[Parameter] public EventCallback OnClearFilter { get; set; }

	private bool _showClearFilter
	{
		get
		{
			if (!String.IsNullOrWhiteSpace(TfAppState.Value.Route.Search)) return true;
			if (TfAppState.Value.SpaceViewFilters is not null && TfAppState.Value.SpaceViewFilters.Count > 0) return true;
			if (TfAppState.Value.SpaceViewSorts is not null && TfAppState.Value.SpaceViewSorts.Count > 0) return true;
			return false;
		}
	}
	private async Task OnFilterClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewFiltersDialog>(
						true,
						new DialogParameters()
						{
							PreventDismissOnOverlayClick = true,
							PreventScroll = true,
							Width = TfConstants.DialogWidthExtraLarge,
			TrapFocus = false
						});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			await OnFilter.InvokeAsync((List<TucFilterBase>)result.Data);
		}
	}

	private async Task OnClearFilterClick()
	{
		await OnClearFilter.InvokeAsync();
	}


}