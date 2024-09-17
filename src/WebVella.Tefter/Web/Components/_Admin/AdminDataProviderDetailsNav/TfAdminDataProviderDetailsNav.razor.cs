namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminDataProviderDetailsNav.TfAdminDataProviderDetailsNav","WebVella.Tefter")]
public partial class TfAdminDataProviderDetailsNav : TfBaseComponent
{
	[Inject] private IState<TfRouteState> TfRouteState { get; set; }

	private List<MenuItem> _getMenu()
	{
		var menu = new List<MenuItem>();
		var providerId = TfRouteState.Value.DataProviderId ?? Guid.Empty;
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderDetailsPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Info(),
			Title = LOC("Details")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderSchemaPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Title = LOC("Columns in source")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderKeysPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Title = LOC("Shared Keys")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderAuxColumnsPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.TableAdd(),
			Title = LOC("Shared Columns")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderSynchronizationPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Database(),
			Title = LOC("Synchronization")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderDataPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Database(),
			Title = LOC("Data")
		});
		return menu;
	}

}