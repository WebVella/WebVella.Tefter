namespace WebVella.Tefter.UI.Components;
public partial class TucAdminUserDetailsAsideToolbar : TfBaseComponent
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

	private async Task addUser()
	{
		var dialog = await DialogService.ShowDialogAsync<TucUserManageDialog>(
		new TfUser(),
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
			var user = (TfUser)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminUserDetailsPageUrl, user.Id));
		}

	}
}