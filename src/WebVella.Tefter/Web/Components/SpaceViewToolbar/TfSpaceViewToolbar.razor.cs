namespace WebVella.Tefter.Web.Components.SpaceViewToolbar;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
    [Inject] protected IState<SessionState> SessionState { get; set; }

    private List<Guid> _selectedRows = new();

    private List<ScreenRegionComponent> _regionComponents = new();
    private long _lastRegionRenderedTimestamp = 0;
    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            SessionState.StateChanged -= SessionState_StateChanged;
		}
        await base.DisposeAsyncCore(disposing);
    }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SessionState.StateChanged += SessionState_StateChanged;
	}

    private void SessionState_StateChanged(object sender, EventArgs e)
    {
        base.InvokeAsync(async () =>
        {
            _selectedRows = SessionState.Value.SelectedDataRows;
            await InvokeAsync(StateHasChanged);
        });
    }

}