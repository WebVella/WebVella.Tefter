namespace WebVella.Tefter.UI.Components;

public partial class TucHomeNavigation : TfBaseComponent, IDisposable
{
	private List<TfMenuItem> _menu = new();

	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override void OnInitialized()
	{
		_menu = TfAuthLayout.GetState().Menu;
		Navigator.LocationChanged += On_NavigationStateChanged;
		EnableRenderLock();
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_menu = TfService.GetAppState(Navigator, TfAuthLayout.GetState().User, Navigator.Uri).Menu;
		RegenRenderLock();
		StateHasChanged();
	}
}