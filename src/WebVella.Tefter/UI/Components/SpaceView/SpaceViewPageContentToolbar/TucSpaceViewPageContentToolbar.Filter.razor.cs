namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar : TfBaseComponent
{
	private bool _showClearFilter
	{
		get
		{
			if (!String.IsNullOrWhiteSpace(_navState.Search)) return true;
			if (_navState.Filters is not null && _navState.Filters.Count > 0) return true;
			if (_navState.Sorts is not null && _navState.Sorts.Count > 0) return true;
			return false;
		}
	}
	private async Task OnFilterClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewFiltersDialog>(
						SpaceView.Id,
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
			await TucSpaceViewPageContent.OnFilter((List<TfFilterQuery>)result.Data);
		}
	}

	private async Task OnClearFilterClick()
	{
		await TucSpaceViewPageContent.OnClearFilter();
	}

}