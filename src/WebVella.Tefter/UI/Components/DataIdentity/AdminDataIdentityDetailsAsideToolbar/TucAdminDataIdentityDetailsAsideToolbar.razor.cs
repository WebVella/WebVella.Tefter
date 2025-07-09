namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataIdentityDetailsAsideToolbar : TfBaseComponent
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

	private async Task addDataIdentity()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataIdentityManageDialog>(
		new TfDataIdentity(),
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
			var item = (TfDataIdentity)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminDataIdentityDetailsPageUrl, item.DataIdentity));
		}

	}
}