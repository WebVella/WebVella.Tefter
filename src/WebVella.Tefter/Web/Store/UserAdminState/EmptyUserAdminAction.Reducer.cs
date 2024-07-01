namespace WebVella.Tefter.Web.Store.UserAdminState;

public static partial class UserAdminStateReducers
{
	[ReducerMethod()]
	public static UserAdminState EmptyUserAdminActionReducer(UserAdminState state, EmptyUserAdminAction action) 
		=> state with { IsBusy = false, User = null};
}
