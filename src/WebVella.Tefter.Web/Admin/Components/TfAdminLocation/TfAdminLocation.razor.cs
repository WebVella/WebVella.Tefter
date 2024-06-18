namespace WebVella.Tefter.Web.Components;
public partial class TfAdminLocation : TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }
	private List<MenuItem> _bcMenu = new();

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= OnLocationChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{

			generateNamedLocation(null);
			StateHasChanged();

			Navigator.LocationChanged += OnLocationChanged;
		}
	}

	protected void OnLocationChanged(object sender, LocationChangedEventArgs args)
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
				Title = TfConstants.AdminDashboardMenuTitle,
				Url = "/admin"
			});
		}
		else if (localPath == "/admin/data-providers")
		{
			_bcMenu.Add(new MenuItem
			{
				Title = TfConstants.AdminDataProvidersMenuTitle,
				Url = "/admin/data-providers"
			});
		}
		else if (localPath == "/admin/users")
		{
			_bcMenu.Add(new MenuItem
			{
				Title = TfConstants.AdminUsersMenuTitle,
				Url = "/admin/users"
			});
		}
	}


	
}