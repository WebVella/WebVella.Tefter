namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceView : TfBaseComponent
{
	private Icon _iconUnselected = TfConstants.GetIcon("CheckboxUnchecked").WithColor(Color.FillInverse);
	private Icon _iconSelected = TfConstants.GetIcon("CheckboxChecked");
	private Icon _iconSelectedIndeterminete = TfConstants.GetIcon("CheckboxIndeterminate");

	private List<DemoDataRow> _selectedItems = new();

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
		Dispatcher.Dispatch(new SetSelectedDataRowsAction(_selectedItems.Select(x => x.Id).ToList()));
	}

	private void onRowSelect(DemoDataRow row)
	{
		if (_selectedItems.Contains(row))
		{
			_selectedItems.Remove(row);
		}
		else
		{
			_selectedItems.Add(row);
		}
		Dispatcher.Dispatch(new SetSelectedDataRowsAction(_selectedItems.Select(x => x.Id).ToList()));
	}
}
