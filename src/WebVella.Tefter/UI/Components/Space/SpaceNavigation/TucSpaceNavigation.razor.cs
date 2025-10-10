namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceNavigation : TfBaseComponent,IDisposable
{
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}
	protected override void OnInitialized()
	{
		Navigator.LocationChanged += On_NavigationStateChanged;
	}
	
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		StateHasChanged();
	}	
}