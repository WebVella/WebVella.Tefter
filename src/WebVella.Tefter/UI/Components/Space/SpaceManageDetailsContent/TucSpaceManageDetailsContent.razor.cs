namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceManageDetailsContent : TfBaseComponent, IDisposable
{
	private TfSpace _space = default!;
	private TfNavigationState _navState = default!;
	private string? _menu = null;
	public bool _submitting = false;
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUIService.SpaceCreated -= On_SpaceChange;
		TfUIService.SpaceUpdated -= On_SpaceChange;
		TfUIService.SpaceDeleted -= On_SpaceChange;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfUIService.SpaceCreated += On_SpaceChange;
		TfUIService.SpaceUpdated += On_SpaceChange;
		TfUIService.SpaceDeleted += On_SpaceChange;
	}


	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async void On_SpaceChange(object? caller, TfSpace args)
	{
		await _init();
	}

	private async Task _init(TfNavigationState? navState = null, TfSpace? role = null)
	{
		if (navState == null)
			_navState = TfAuthLayout.NavigationState;
		else
			_navState = navState;

		try
		{
			if (_navState.SpaceId is null) return;
			_space = TfUIService.GetSpace(_navState.SpaceId.Value);
			if (_space is null) return;

			_menu = null;
			if(_navState.NodesDict.Keys.Count > 3){ 
				_menu = _navState.NodesDict[3];
			}
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editSpace()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceManageDialog>(
		_space,
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

	private async Task _deleteSpace()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space deleted?")))
			return;
		try
		{
			TfUIService.DeleteSpace(_space.Id);
			ToastService.ShowSuccess(LOC("Space deleted"));
			Navigator.NavigateTo(TfConstants.HomePageUrl);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
}