﻿namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderDetailsNav.TfAdminDataProviderDetailsNav","WebVella.Tefter")]
public partial class TfAdminDataProviderDetailsNav : TfBaseComponent
{
	[Inject] private IState<TfRouteState> TfRouteState { get; set; }

	private List<TucMenuItem> _getMenu()
	{
		var menu = new List<TucMenuItem>();
		var providerId = TfRouteState.Value.DataProviderId ?? Guid.Empty;
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderDetailsPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Info(),
			Title = LOC("Details")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderSchemaPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Title = LOC("Columns")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderKeysPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Title = LOC("Shared Keys")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderAuxColumnsPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.TableAdd(),
			Title = LOC("Shared Columns")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderSynchronizationPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Database(),
			Title = LOC("Synchronization")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderDataPageUrl, providerId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Database(),
			Title = LOC("Data")
		});
		return menu;
	}

}