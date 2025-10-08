namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceManageDetailsContent : TfBaseComponent, IDisposable
{
	private TfSpace _space = null!;
	private TfNavigationState _navState = null!;
	private string? _menu = null;
	public bool _submitting = false;
	public void Dispose()
	{
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
		TfEventProvider.SpaceCreatedEvent -= On_SpaceChange;
		TfEventProvider.SpaceUpdatedEvent -= On_SpaceChange;
		TfEventProvider.SpaceDeletedEvent -= On_SpaceChange;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfState.NavigationState);
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
		TfEventProvider.SpaceCreatedEvent += On_SpaceChange;
		TfEventProvider.SpaceUpdatedEvent += On_SpaceChange;
		TfEventProvider.SpaceDeletedEvent += On_SpaceChange;
	}


	private async Task On_NavigationStateChanged(TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(navState: args);
		});
	}

	private async Task On_SpaceChange(object args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfState.NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState, TfSpace? role = null)
	{
		_navState = navState;

		try
		{
			if (_navState.SpaceId is null) return;
			_space = TfService.GetSpace(_navState.SpaceId.Value);
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