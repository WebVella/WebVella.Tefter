namespace WebVella.Tefter.Web.Store;

public static partial class UserAdminStateReducers
{
	[ReducerMethod()]
	public static TfAppState SetUserAdminActionReducer(TfAppState state, SetUserAdminAction action) 
		=> state with {ManagedUser = action.ManagedUser};
}
