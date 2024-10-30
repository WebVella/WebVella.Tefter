namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderNavigation.TfAdminDataProviderNavigation", "WebVella.Tefter")]
public partial class TfAdminDataProviderNavigation : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
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
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderManageDialog>(new TucDataProvider(), new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var provider = (TucDataProvider)result.Data;
			Navigator.NavigateTo(String.Format(TfConstants.AdminDataProviderDetailsPageUrl, provider.Id));
		}
	}
	private List<TucDataProvider> _getProviders()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucDataProvider>();
		foreach (var provider in TfAppState.Value.AdminDataProviders)
		{
			if (!String.IsNullOrWhiteSpace(search) &&
				!(provider.Name.ToLowerInvariant().Contains(search))
				)
				continue;

			menuItems.Add(provider);
		}

		return menuItems;
	}

	private void onSearch(string search)
	{
		this.search = search;
	}
}