namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceHeader : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;

	private TfSpace? _space = null!;
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider?.Dispose();
	}
	protected override void OnInitialized()
	{
		_space = TfAuthLayout.GetState().Space;
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.SpaceUpdatedEvent += On_SpaceUpdated;
	}
	
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		StateHasChanged();
	}	
	
	private async Task On_SpaceUpdated(TfSpaceUpdatedEvent args)
	{
		_space = args.Payload;
		await InvokeAsync(StateHasChanged);
	}		
		
}
