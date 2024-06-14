namespace WebVella.Tefter.Web.Components;
public partial class TfLocation : TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }
	private bool _settingsMenuVisible = false;
	private int _ellipsisCount = 20;
	private MenuItem _namedLocation = null;

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

			_namedLocation = generateNamedLocation();
			StateHasChanged();

			Navigator.LocationChanged += OnLocationChanged;
		}
	}

	protected void OnLocationChanged(object sender, LocationChangedEventArgs args)
	{

		base.InvokeAsync(async () =>
		{
			_namedLocation = generateNamedLocation();
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


	private void onDetailsClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for details about the space item");
	}
	private void onRemoveClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for removing space item");
	}
	private void onRenameClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for renaming");
	}

	private void onAccessClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Shows current access");
	}

	private void onBookmarkClick()
	{
		//WvState.SetSpaceViewBookmarkState(_spaceView.Id, !_spaceView.IsBookmarked);
	}
}