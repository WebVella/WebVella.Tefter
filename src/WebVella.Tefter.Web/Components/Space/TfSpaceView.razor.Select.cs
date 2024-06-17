namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceView : TfBaseComponent
{
	private Icon _iconUnselected = new Icons.Regular.Size20.CheckboxUnchecked().WithColor(Color.FillInverse);
	private Icon _iconSelected = new Icons.Filled.Size20.CheckboxChecked();
	private Icon _iconSelectedIndeterminete = new Icons.Filled.Size20.CheckboxIndeterminate();

	private List<DataRow> _selectedItems = new();

	private void _onMultipleSelect()
	{
		if (_selectedItems.Count > 0)
		{
			_selectedItems.Clear();
		}
		else
		{
			_selectedItems.AddRange(_data);
		}
	}

	private void onRowSelect(DataRow row)
	{
		if (_selectedItems.Contains(row))
		{
			_selectedItems.Remove(row);
		}
		else
		{
			_selectedItems.Add(row);
		}
	}
}
