
namespace WebVella.Tefter.Web.Components.AdminDataProviderDetailsNav;
public partial class TfAdminDataProviderDetailsNav : TfBaseComponent
{
	private List<MenuItem> menu = new();

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}


	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			GenerateMenu();
			ActionSubscriber.SubscribeToAction<DataProviderDetailsChangedAction>(this, On_GetDataProviderDetailsActionResult);
			StateHasChanged();
		}
	}
	private void GenerateMenu()
	{
		menu.Clear();
		var providerId = Navigator.GetUrlData().DataProviderId ?? Guid.Empty;
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderDetailsPageUrl, providerId),
			Match = NavLinkMatch.All,
			Icon = new Icons.Regular.Size20.Info(),
			Title = LOC("Details")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderSchemaPageUrl, providerId),
			Match = NavLinkMatch.All,
			Icon = new Icons.Regular.Size20.Table(),
			Title = LOC("Schema")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderAuxColumnsPageUrl, providerId),
			Match = NavLinkMatch.All,
			Icon = new Icons.Regular.Size20.TableAdd(),
			Title = LOC("Additional Columns")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminDataProviderRecordsPageUrl, providerId),
			Match = NavLinkMatch.All,
			Icon = new Icons.Regular.Size20.Database(),
			Title = LOC("Data")
		});

	}

	private void On_GetDataProviderDetailsActionResult(DataProviderDetailsChangedAction action)
	{
		GenerateMenu();
		StateHasChanged();
	}
}