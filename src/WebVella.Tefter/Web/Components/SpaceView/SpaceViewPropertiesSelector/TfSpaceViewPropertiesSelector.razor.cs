namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewPropertiesSelector : TfBaseComponent
{
	//[Parameter]
	//public EventCallback<GridPropertiesChangedEventArgs> ColumnChanged { get; set; }

    [Inject] protected IState<TfAppState> TfAppState { get; set; }

	private bool _open = false;

	private void _init()
	{
	}

	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}
}