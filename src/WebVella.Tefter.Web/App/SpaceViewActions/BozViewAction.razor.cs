using Boz.Tefter.Components;

namespace Boz.Tefter.Actions;
public partial class BozViewAction : TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }
	private List<Guid> _selectedItems = new List<Guid>();
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			SessionState.StateChanged -= SessionState_StateChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
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
			_selectedItems = SessionState.Value.SelectedDataRows.ToList();
			await InvokeAsync(StateHasChanged);
		});

	}


	private void onClick(){
        ToastService.ShowToast(ToastIntent.Warning, "Hello from Boz App");

    }
}