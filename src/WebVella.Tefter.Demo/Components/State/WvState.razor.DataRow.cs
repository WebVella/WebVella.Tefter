namespace WebVella.Tefter.Demo.Components;
public partial class WvState : ComponentBase
{

	private IEnumerable<DataRow> _selectedRows = Enumerable.Empty<DataRow>();

	public EventHandler<StateSelectionChangedEventArgs> SelectedDataRowsChanged { get; set; }
	public IEnumerable<DataRow> GetSelectedRows() => _selectedRows;
	public void SetSelectedRows(IEnumerable<DataRow> rows)
	{
		_selectedRows = rows;
		SelectedDataRowsChanged?.Invoke(this,new StateSelectionChangedEventArgs{ Rows = _selectedRows });
	}

}
