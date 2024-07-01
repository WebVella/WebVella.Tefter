namespace WebVella.Tefter.Web.Store.UserAdminState;

public static partial class UserAdminStateReducers
{
	[ReducerMethod()]
	public static UserAdminState SetUserAdminActionReducer(UserAdminState state, SetUserAdminAction action) 
		=> state with { IsBusy = action.IsBusy, User = action.User};
}
