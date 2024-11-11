namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderDetailsNav.TfAdminDataProviderDetailsNav","WebVella.Tefter")]
public partial class TfAdminDataProviderDetailsNav : TfBaseComponent
{
	[Inject] private IState<TfAppState> TfAppState { get; set; }

	private List<TucMenuItem> _getMenu()
	{
		var menu = new List<TucMenuItem>();
		if(TfAppState.Value.Route?.DataProviderId is null) return menu;
		var providerId = TfAppState.Value.Route.DataProviderId.Value;
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderDetailsPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Info(),
			Text = LOC("Details")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderSchemaPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Text = LOC("Columns")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderAuxColumnsPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.TableAdd(),
			Text = LOC("Shared Columns")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderKeysPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Text = LOC("Shared Keys")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderSynchronizationPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Database(),
			Text = LOC("Synchronization")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderDataPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Database(),
			Text = LOC("Data")
		});
		return menu;
	}

}