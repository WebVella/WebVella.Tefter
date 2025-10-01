namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewManageTabPageContent : TfBaseComponent, IDisposable
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
		await _init(Navigator.GetRouteState());
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
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
			{
				await _init(args);
			}
		});
	}

	private async void On_SpaceViewUpdated(object? caller, List<TfSpaceViewColumn> args)
	{
		await _init(null);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_isDataLoading = true;
			await InvokeAsync(StateHasChanged);
			_navState = navState;
			_spaceView = null;
			var options =
				JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(Context!.SpacePage.ComponentOptionsJson);
			if (options is null || options.SpaceViewId is null)
				return;
			_spaceView = TfUIService.GetSpaceView(options.SpaceViewId.Value);
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