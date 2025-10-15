namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceManageDetailsContent : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private TfSpace _space = null!;
	private TfNavigationState _navState = null!;
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider.Dispose();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.SpaceCreatedEvent += On_SpaceChange;
		TfEventProvider.SpaceUpdatedEvent += On_SpaceChange;
		TfEventProvider.SpaceDeletedEvent += On_SpaceChange;
	}


	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_SpaceChange(object args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;

		try
		{
			if (_navState.SpaceId is null) return;
			_space = TfService.GetSpace(_navState.SpaceId.Value);
			if (_space is null) return;
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
		new ()
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
			TfService.DeleteSpace(_space.Id);
			ToastService.ShowSuccess(LOC("Space deleted"));
			Navigator.NavigateTo(TfConstants.HomePageUrl);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
}