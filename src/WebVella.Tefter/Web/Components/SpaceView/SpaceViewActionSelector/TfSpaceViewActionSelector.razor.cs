namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewActionSelector : TfBaseComponent
{
    [Inject] protected IState<TfAppState> TfAppState { get; set; }

	
	private bool _open = false;
	private bool _selectorLoading = false;
	private List<Guid> _selectedItems = new List<Guid>();

	private List<ScreenRegionComponent> _regionComponents = new();
	private long _lastRegionRenderedTimestamp = 0;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_StateChanged);
	}

	private void On_StateChanged(SpaceStateChangedAction action)
	{
		InvokeAsync(async () =>
		{
			_selectedItems = TfAppState.Value.SelectedDataRows.ToList();
			await InvokeAsync(StateHasChanged);
		});

	}


	private void _init(){ }

	public async Task ToggleSelector()
	{
		_open = !_open;
		if (_open)
		{
			_selectorLoading = true;
			await InvokeAsync(StateHasChanged);
			_init();

			_selectorLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}