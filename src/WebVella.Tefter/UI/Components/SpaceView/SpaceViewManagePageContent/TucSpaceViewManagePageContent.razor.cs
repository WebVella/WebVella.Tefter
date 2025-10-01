namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewManagePageContent : TfBaseComponent, IDisposable
{
	#region << Init >>
	[Parameter] public TfSpacePageAddonContext? Context { get; set; } = null;

	// State
	private bool _isDataLoading = true;
	private TfNavigationState _navState = null!;
	private TfSpaceView? _spaceView = null;
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUIService.SpaceViewColumnsChanged -= On_SpaceViewUpdated;
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Context is null)
			throw new Exception("Context cannot be null");		
		await _init();
		_isDataLoading = false;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			TfUIService.NavigationStateChanged += On_NavigationStateChanged;
			TfUIService.SpaceViewColumnsChanged += On_SpaceViewUpdated;
		}
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
		{
			await _init(navState: args);
		}
	}
	private async void On_SpaceViewUpdated(object? caller, List<TfSpaceViewColumn> args)
	{
		await _init(null);
	}

	private async Task _init(TfNavigationState? navState = null)
	{

		try
		{
			_isDataLoading = true;
			await InvokeAsync(StateHasChanged);

			if (navState == null)
				_navState = TfAuthLayout.NavigationState;
			else
				_navState = navState;

			Guid? oldViewId = _spaceView is not null ? _spaceView.Id : null;
			_spaceView = null;
			if (_navState.SpaceId is null || _navState.SpacePageId is null)
				return;

		}
		finally
		{
			_isDataLoading = false;
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	#endregion

}