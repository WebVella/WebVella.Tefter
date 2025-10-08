namespace WebVella.Tefter.UI.Components;

public partial class TucPageSearch : TfBaseComponent, IDisposable
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

	private async Task onSearch(string search)
	{
		await NavigatorExt.ApplyChangeToUrlQuery(Navigator, TfConstants.SearchQueryName, search);
	}
}