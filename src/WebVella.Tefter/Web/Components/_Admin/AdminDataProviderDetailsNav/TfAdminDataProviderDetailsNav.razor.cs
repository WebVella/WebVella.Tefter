namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminDataProviderDetailsNav.TfAdminDataProviderDetailsNav","WebVella.Tefter")]
public partial class TfAdminDataProviderDetailsNav : TfBaseComponent
{
	private List<MenuItem> menu = new();

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		GenerateMenu();
		//ActionSubscriber.SubscribeToAction<DataProviderAdminChangedAction>(this, On_GetDataProviderDetailsActionResult);
		Navigator.LocationChanged += Navigator_LocationChanged;
		StateHasChanged();
	}

	private void GenerateMenu()
	{
		menu.Clear();
		var providerId = Navigator.GetRouteState().DataProviderId ?? Guid.Empty;
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

	}

	//private void On_GetDataProviderDetailsActionResult(DataProviderAdminChangedAction action)
	//{
	//	GenerateMenu();
	//	StateHasChanged();
	//}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		GenerateMenu();
		StateHasChanged();
	}

}