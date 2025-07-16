namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceDataDetailsAsideToolbar : TfBaseComponent
{

	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	private string? _search = null;

	protected override void OnInitialized()
	{
		_search = NavigatorExt.GetStringFromQuery(Navigator, TfConstants.AsideSearchQueryName, null);
	}

	private async Task onSearch(string search)
	{
		_search = search;
		await NavigatorExt.ApplyChangeToUrlQuery(Navigator, TfConstants.AsideSearchQueryName, search);
	}

	private async Task addSpaceData()
	{
		var navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		if(navState.SpaceId is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpaceDataManageDialog>(
		new TfSpaceData(){ SpaceId = navState.SpaceId.Value},
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
			var item = (TfSpaceData)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.SpaceDataPageUrl, navState.SpaceId.Value, item.Id));
		}

	}
}