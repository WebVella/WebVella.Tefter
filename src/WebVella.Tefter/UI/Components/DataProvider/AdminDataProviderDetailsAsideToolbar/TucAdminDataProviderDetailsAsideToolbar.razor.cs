namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderDetailsAsideToolbar : TfBaseComponent
{
	private string? _search = null;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_search = NavigatorExt.GetStringFromQuery(Navigator,TfConstants.AsideSearchQueryName,null);
	}

	private async Task onSearch(string search)
	{
		_search = search;
		await NavigatorExt.ApplyChangeToUrlQuery(Navigator,TfConstants.AsideSearchQueryName,search);
	}

	private async Task addProvider()
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
			var item = (TfDataProvider)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminDataProviderDetailsPageUrl, item.Id));
		}

	}
}