namespace WebVella.Tefter.Web.Store.UserAdminState;

public static partial class UserAdminStateReducers
{
	[ReducerMethod()]
	public static UserAdminState GetUserAdminActionReducer(UserAdminState state, GetUserAdminAction action) 
		=> state with { IsBusy = true};
}
