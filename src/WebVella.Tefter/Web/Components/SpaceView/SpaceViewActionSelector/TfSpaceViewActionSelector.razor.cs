namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewActionSelector : TfBaseComponent
{
    [Inject] protected IState<TfAppState> TfAppState { get; set; }

	
	private bool _open = false;
	private bool _selectorLoading = false;

	private List<ScreenRegionComponent> _regionComponents = new();
	private long _lastRegionRenderedTimestamp = 0;


	public async Task ToggleSelector()
	{
		_open = !_open;
		if (_open)
		{
			_selectorLoading = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1000); //load components with actions?
			_selectorLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}