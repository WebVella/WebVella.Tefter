namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataIdentityNavigation.TfAdminDataIdentityNavigation", "WebVella.Tefter")]
public partial class TfAdminDataIdentityNavigation : TfBaseComponent
{
	[Inject] protected IStateSelection<TfUserState,bool> SidebarExpanded { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private string search = null;
	private int _stringLimit = 30;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataIdentityManageDialog>(
			new TucDataIdentity(), new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucDataIdentity)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminDataIdentityDetailsPageUrl, item.Name));
		}
	}
	private List<TucDataIdentity> _getIdentities()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucDataIdentity>();
		foreach (var identity in TfAppState.Value.AdminDataIdentities)
		{
			if (!String.IsNullOrWhiteSpace(search) &&
				!(identity.Description.ToLowerInvariant().Contains(search))
				)
				continue;

			menuItems.Add(identity);
		}

		return menuItems;
	}

	private void onSearch(string search)
	{
		this.search = search;
	}
}