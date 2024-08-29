namespace WebVella.Tefter.Web.Components.SpaceViewToolbar;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
    [Inject] protected IState<SpaceState> SpaceState { get; set; }

    private List<Guid> _selectedRows = new();

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
            _selectedRows = SpaceState.Value.SelectedDataRows;
            await InvokeAsync(StateHasChanged);
		});

	}



}