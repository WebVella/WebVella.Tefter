namespace WebVella.Tefter.UI.Components;
public partial class TucAdminRoleDetailsAsideToolbar : TfBaseComponent
{
	private string? _search = null;

	protected override void OnInitialized()
	{
		_search = NavigatorExt.GetStringFromQuery(Navigator,TfConstants.AsideSearchQueryName,null);
	}

	private async Task onSearch(string search)
	{
		_search = search;
		await NavigatorExt.ApplyChangeToUrlQuery(Navigator,TfConstants.AsideSearchQueryName,search);
	}

	private async Task addRole()
	{
		var dialog = await DialogService.ShowDialogAsync<TucRoleManageDialog>(
		new TfRole(),
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
			var role = (TfRole)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminRoleDetailsPageUrl, role.Id));
		}

	}
}