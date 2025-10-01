namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewDetailsAsideToolbar : TfBaseComponent, IDisposable
{
	private string _backUrl = "#";
	private TfNavigationState _navState = new();
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(Navigator.GetRouteState());
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;

		try
		{
			_backUrl = "#";
			if (_navState.SpaceId is null || _navState.SpaceViewId is null) return;

			var spacePage = TfUIService.GetSpacePageBySpaceViewId(_navState.SpaceViewId.Value);
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