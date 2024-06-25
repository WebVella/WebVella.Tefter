namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static SessionState SetSelectedDataRowsReducer(SessionState state, SetSelectedDataRowsAction action)
	{
		return state with { SelectedDataRows = action.SelectedRows };
	}
}