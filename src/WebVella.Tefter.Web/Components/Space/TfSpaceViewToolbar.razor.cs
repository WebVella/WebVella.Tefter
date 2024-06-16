namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private int _selectedRowsCount = 0;
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			SessionState.StateChanged -= SessionState_StateChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			SessionState.StateChanged += SessionState_StateChanged;
		}
	}

	private void SessionState_StateChanged(object sender, EventArgs e)
	{
		base.InvokeAsync(async () =>
		{
			//Do something
			
			await InvokeAsync(StateHasChanged);
		});
	}

}