namespace WebVella.Tefter.Web.Store.SessionState;

public class SetSelectedDataRowsAction
{
    public List<Guid> SelectedRows { get; } = new();

    public SetSelectedDataRowsAction(List<Guid> selectedRows)
    {
		SelectedRows = selectedRows;
    }
}
