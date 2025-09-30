namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewDetailsAsideToolbar : TfBaseComponent, IDisposable
{

	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;

	private string _backUrl = "#";
	private TfNavigationState _navState = new();
	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;

		try
		{
			_backUrl = "#";
			if (_navState.SpaceId is null || _navState.SpaceViewId is null) return;

			var spacePage = TfSpaceUIService.GetSpacePageBySpaceViewId(_navState.SpaceViewId.Value);
			if (spacePage is not null)
			{
				_backUrl = String.Format(TfConstants.SpacePagePageUrl, spacePage.SpaceId, spacePage.Id);
			}
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}