namespace WebVella.Tefter.Web.Components.Location;
public partial class TfLocation : TfBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	private bool _settingsMenuVisible = false;
	private int _ellipsisCount = 30;
	private MenuItem _namedLocation = null;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_namedLocation = generateNamedLocation();
		Navigator.LocationChanged += Navigator_LocationChanged;
		ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_SpaceChangedAction);
	}

	protected void Navigator_LocationChanged(object sender, LocationChangedEventArgs args)
	{

		base.InvokeAsync(async () =>
		{
			_namedLocation = generateNamedLocation();
			await InvokeAsync(StateHasChanged);
		});
	}

	private void On_SpaceChangedAction(SpaceStateChangedAction action)
	{
		base.InvokeAsync(async () =>
		{
			await InvokeAsync(StateHasChanged);
		});
	}

	private MenuItem generateNamedLocation()
	{
		MenuItem location = null;
		var uri = new Uri(Navigator.Uri);
		var localPath = uri.LocalPath.ToLowerInvariant();
		if (localPath == "/")
		{
			location = new MenuItem
			{
				Title = TfConstants.DashboardMenuTitle,
				Icon = TfConstants.DashboardIcon,
				Url = "/"
			};
		}
		else if (localPath == "/fast-access")
		{
			location = new MenuItem
			{
				Title = "Bookmarked Views",
				Icon = TfConstants.BookmarkONIcon,
				Url = "/fast-access"
			};
		}
		else if (localPath == "/fast-access/saves")
		{
			location = new MenuItem
			{
				Title = "Saved Views",
				Icon = TfConstants.SaveIcon,
				Url = "/fast-access/saves"
			};
		}

		return location;
	}
}