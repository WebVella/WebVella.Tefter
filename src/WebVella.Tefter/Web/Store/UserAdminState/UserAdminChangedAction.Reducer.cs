namespace WebVella.Tefter.Web.Store.UserAdminState;

public static partial class UserAdminStateReducers
{
	[ReducerMethod()]
	public static UserAdminState GetUserAdminActionResultReducer(UserAdminState state, UserAdminChangedAction action)
		=> state with { IsBusy = action.IsBusy, User = action.User };
}
