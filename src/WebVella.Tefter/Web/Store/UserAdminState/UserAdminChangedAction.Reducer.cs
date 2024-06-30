namespace WebVella.Tefter.Web.Store.UserAdminState;

public static partial class UserAdminStateReducers
{
	[ReducerMethod()]
	public static UserAdminState GetUserAdminActionResultReducer(UserAdminState state, UserAdminChangedAction action)
		=> state with { IsBusy = false, User = action.User };
}
