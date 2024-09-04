namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminLocation.TfAdminLocation","WebVella.Tefter")]
public partial class TfAdminLocation : TfBaseComponent
{
	private List<MenuItem> _bcMenu = new();

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		generateNamedLocation(null);
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	protected void Navigator_LocationChanged(object sender, LocationChangedEventArgs args)
	{

		base.InvokeAsync(async () =>
		{
			generateNamedLocation(args.Location);
			await InvokeAsync(StateHasChanged);
		});
	}

	private void generateNamedLocation(string location)
	{
		_bcMenu.Clear();
		var url = String.IsNullOrWhiteSpace(location) ? Navigator.Uri : location;
		var uri = new Uri(url);
		var localPath = uri.LocalPath.ToLowerInvariant();
		if (localPath == "/admin")
		{
			_bcMenu.Add(new MenuItem
			{
				Title = LOC(TfConstants.AdminDashboardMenuTitle),
				Url = "/admin"
			});
		}
		else if (localPath.StartsWith("/admin/data-providers"))
		{
			_bcMenu.Add(new MenuItem
			{
				Title = LOC(TfConstants.AdminDataProvidersMenuTitle),
				Url = "/admin/data-providers"
			});
		}
		else if (localPath.StartsWith("/admin/users"))
		{
			_bcMenu.Add(new MenuItem
			{
				Title = LOC(TfConstants.AdminUsersMenuTitle),
				Url = "/admin/users"
			});
		}
		else if (localPath.StartsWith("/admin/aux-columns"))
		{
			_bcMenu.Add(new MenuItem
			{
				Title = LOC(TfConstants.AdminAuxColumnsMenuTitle),
				Url = "/admin/aux-columns"
			});
		}
	}


	
}