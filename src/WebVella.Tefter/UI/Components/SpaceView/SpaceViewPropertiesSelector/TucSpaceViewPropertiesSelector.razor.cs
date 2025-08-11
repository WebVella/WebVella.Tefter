namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPropertiesSelector : TfBaseComponent
{
	//[Parameter]
	//public EventCallback<GridPropertiesChangedEventArgs> ColumnChanged { get; set; }
	[Parameter] public TfSpaceView SpaceView { get; set; } = default!;
	[Parameter] public TfDataTable Data { get; set; } = default!;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();

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