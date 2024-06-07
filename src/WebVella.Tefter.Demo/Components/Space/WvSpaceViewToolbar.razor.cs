

namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceViewToolbar : WvBaseComponent
{
	private IEnumerable<DataRow> _selectedRows = Enumerable.Empty<DataRow>();
	private int _selectedRowsCount = 0;
	public override async ValueTask DisposeAsync()
	{
		WvState.SelectedDataRowsChanged -= _selectedRowsChanged;
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			WvState.SelectedDataRowsChanged += _selectedRowsChanged;
		}
	}

	private void _selectedRowsChanged(object sender, StateSelectionChangedEventArgs e)
	{
		base.InvokeAsync(async () =>
		{
			_selectedRows = e.Rows;
			_selectedRowsCount = _selectedRows.Count();
			await InvokeAsync(StateHasChanged);
		});
	}

}