namespace WebVella.Tefter.Web.Store.UserDetailsState;

public static partial class UserDetailsStateReducers
{
	[ReducerMethod()]
	public static UserDetailsState GetUserDetailsActionResultReducer(UserDetailsState state, GetUserDetailsActionResult action)
		=> state with { IsBusy = false, User = action.User };
}
