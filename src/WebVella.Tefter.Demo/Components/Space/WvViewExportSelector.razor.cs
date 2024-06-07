
namespace WebVella.Tefter.Demo.Components;
public partial class WvViewExportSelector : WvBaseComponent
{
	[Parameter]
	public IEnumerable<DataRow> Rows { get; set; } = Enumerable.Empty<DataRow>();

	[Parameter]
	public int RowsCount { get; set; } = 0;

	[Parameter]
	public EventCallback<SpaceViewExportChangedEventArgs> ExportChanged { get; set; }


	private bool _open = false;
	private bool _selectorLoading = false;

	private void _init()
	{
	}

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