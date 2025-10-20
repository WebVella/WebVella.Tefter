namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceNavigation : TfBaseComponent,IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private List<TfMenuItem> _menu = new();
	
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider.Dispose();
	}
	protected override void OnInitialized()
	{
		_menu = TfAuthLayout.GetState().Menu;
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.SpaceUpdatedEvent += On_SpaceOrPageUpdated;
		TfEventProvider.SpacePageCreatedEvent += On_SpaceOrPageUpdated;
		TfEventProvider.SpacePageUpdatedEvent += On_SpaceOrPageUpdated;
		TfEventProvider.SpacePageDeletedEvent += On_SpaceOrPageUpdated;
	}
	
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_menu = TfService.GetAppState(Navigator, TfAuthLayout.GetState().User, Navigator.Uri, null).Menu;
		StateHasChanged();
	}	
	private async Task On_SpaceOrPageUpdated(object args)
	{
		_menu = TfService.GetAppState(Navigator, TfAuthLayout.GetState().User, Navigator.Uri, null).Menu;
		await InvokeAsync(StateHasChanged);
	}

}