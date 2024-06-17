
namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewActionSelector : TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState {  get; set; }

	private bool _open = false;
	private bool _selectorLoading = false;
	private List<Guid> _selectedItems = new List<Guid>();	

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
			//Do something
			_selectedItems = SessionState.Value.SelectedDataRows.ToList();
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