namespace WebVella.Tefter.Demo.Components;
public partial class WvLocation : WvBaseComponent
{
	private Space _space;
	private SpaceDataset _spaceData;
	private SpaceView _spaceView;
	private bool _settingsMenuVisible = false;
	private int _ellipsisCount = 20;
	private MenuItem _namedLocation = null;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			WvState.ActiveSpaceDataChanged -= OnSpaceDataChanged;
			Navigator.LocationChanged -= OnLocationChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			var meta = WvState.GetActiveSpaceMeta();
			_space = meta.Space;
			_spaceData = meta.SpaceData;
			_spaceView = meta.SpaceView;

			_namedLocation = generateNamedLocation();
			StateHasChanged();

			WvState.ActiveSpaceDataChanged += OnSpaceDataChanged;
			Navigator.LocationChanged += OnLocationChanged;
		}
	}

	protected void OnSpaceDataChanged(object sender, StateActiveSpaceDataChangedEventArgs args)
	{
		base.InvokeAsync(async () =>
		{
			_space = args.Space;
			_spaceData = args.SpaceData;
			_spaceView = args.SpaceView;
			await InvokeAsync(StateHasChanged);
		});
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
				Title = WvConstants.DashboardMenuTitle,
				Icon = WvConstants.DashboardIcon,
				Url = "/"
			};
		}
		else if (localPath == "/fast-access")
		{
			location = new MenuItem
			{
				Title = "Bookmarked Views",
				Icon = WvConstants.BookmarkONIcon,
				Url = "/fast-access"
			};
		}
		else if (localPath == "/fast-access/saves")
		{
			location = new MenuItem
			{
				Title = "Saved Views",
				Icon = WvConstants.SaveIcon,
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
		WvState.SetSpaceViewBookmarkState(_spaceView.Id, !_spaceView.IsBookmarked);
	}

}