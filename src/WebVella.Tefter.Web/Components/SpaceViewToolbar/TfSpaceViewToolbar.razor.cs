namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
    [Inject] protected IState<SessionState> SessionState { get; set; }
    [Inject] protected IState<ScreenState> ScreenState { get; set; }

    private List<Guid> _selectedRows = new();

    private List<ScreenRegionComponent> _regionComponents = new();
    private long _lastRegionRenderedTimestamp = 0;
    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            SessionState.StateChanged -= SessionState_StateChanged;
			ScreenState.StateChanged -= ScreenState_StateChanged;
		}
        await base.DisposeAsyncCore(disposing);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
			await initRegionAsync();
			await InvokeAsync(StateHasChanged);
			SessionState.StateChanged += SessionState_StateChanged;
            ScreenState.StateChanged += ScreenState_StateChanged;
        }
    }

    private void SessionState_StateChanged(object sender, EventArgs e)
    {
        base.InvokeAsync(async () =>
        {
            _selectedRows = SessionState.Value.SelectedDataRows;
            await InvokeAsync(StateHasChanged);
        });
    }

    private void ScreenState_StateChanged(object sender, EventArgs e)
    {
        base.InvokeAsync(async () =>
        {
            await initRegionAsync();
        });
    }

    private async Task initRegionAsync()
    {
        if (_lastRegionRenderedTimestamp < ScreenState.Value.SpaceViewActionsRegionTimestamp)
        {
            _lastRegionRenderedTimestamp = ScreenState.Value.SpaceViewActionsRegionTimestamp;
            _regionComponents = ScreenState.Value.SpaceViewActionsRegion;
            await InvokeAsync(StateHasChanged);
        }
    }

}