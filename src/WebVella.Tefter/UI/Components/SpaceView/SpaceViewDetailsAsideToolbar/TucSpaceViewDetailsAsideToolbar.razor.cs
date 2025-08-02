namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewDetailsAsideToolbar : TfBaseComponent
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

	private async Task addSpaceView()
	{
		var navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		if(navState.SpaceId is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewManageDialog>(
		new TfSpaceView(){ SpaceId = navState.SpaceId.Value},
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
			var item = (TfSpaceView)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.SpaceViewPageUrl, navState.SpaceId.Value, item.Id));
		}

	}
}