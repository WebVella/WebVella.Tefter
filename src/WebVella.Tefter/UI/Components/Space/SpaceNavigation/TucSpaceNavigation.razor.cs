namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceNavigation : TfBaseComponent,IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private List<TfMenuItem> _menu = new();
	
	private string _dragClass = String.Empty;
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider?.Dispose();
	}
	protected override void OnInitialized()
	{
		_menu = TfAuthLayout.GetState().Menu;
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.SpaceUpdatedEvent += On_SpaceOrPageUpdated;
		TfEventProvider.SpacePageUpdatedEvent += On_SpaceOrPageUpdated;
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

	private void _dragEnter()
	{
		_dragClass = "tf-drag-container--dragging";
		StateHasChanged();	

	}

	private void _dragLeave()
	{
		_dragClass = string.Empty;
		StateHasChanged();	
		
	}

}