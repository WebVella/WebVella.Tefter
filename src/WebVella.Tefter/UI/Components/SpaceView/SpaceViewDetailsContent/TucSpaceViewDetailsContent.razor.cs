namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewDetailsContent : TfBaseComponent, IDisposable
{
	private TfSpaceView _spaceView = new();
	private TfDataset _spaceData = new();
	private TfSpace _space = new();
	private TfDataProvider _provider = new();
	public bool _submitting = false;
	public TfNavigationState? _navState = null;

	public void Dispose()
	{
		TfUIService.SpaceViewUpdated -= On_SpaceViewUpdated;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.NavigationState);
		TfUIService.SpaceViewUpdated += On_SpaceViewUpdated;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_SpaceViewUpdated(object? caller, TfSpaceView args)
	{
		await _init(navState:TfAuthLayout.NavigationState,spaceView: args);
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState navState, TfSpaceView? spaceView = null)
	{
		_navState = navState;
		try
		{
			if (spaceView is not null && spaceView.Id == _spaceView?.Id)
			{
				_spaceView = spaceView;
			}
			else
			{
				var routeData = TfAuthLayout.NavigationState;
				if (routeData.SpaceViewId is not null)
					_spaceView = TfUIService.GetSpaceView(routeData.SpaceViewId.Value);

			}
			if (_spaceView is null) return;
			_space = TfUIService.GetSpace(_spaceView.SpaceId);
			_spaceData = TfUIService.GetDataset(_spaceView.DatasetId);
			_provider = TfUIService.GetDataProvider(_spaceData.DataProviderId);
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editSpaceView()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewManageDialog>(
		_spaceView,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}

	private async Task _deleteSpaceView()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space view deleted?")))
			return;
		try
		{
			//TfUIService.DeleteSpaceView(_spaceView.Id);
			var allSpaceView = TfUIService.GetSpaceViewsList(_navState!.SpaceId!.Value);
			ToastService.ShowSuccess(LOC("Space view deleted"));
			if (allSpaceView.Count > 0)
			{
				Navigator.NavigateTo(string.Format(TfConstants.SpaceViewPageUrl, _navState.SpaceId, allSpaceView[0].Id));
			}
			else{ 
				Navigator.NavigateTo(TfConstants.HomePageUrl);
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private void _copySpaceView()
	{
		try
		{
			//TfUIService.CopySpaceView(_spaceView.Id);
			ToastService.ShowSuccess(LOC("Space view copied"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
}