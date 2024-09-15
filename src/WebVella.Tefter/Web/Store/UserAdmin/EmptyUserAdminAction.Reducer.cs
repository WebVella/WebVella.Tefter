namespace WebVella.Tefter.Web.Store;

public static partial class UserAdminStateReducers
{
	[ReducerMethod()]
	public static TfState EmptyUserAdminActionReducer(TfState state, EmptyUserAdminAction action) 
		=> state with { ManagedUser = action.User};
}
