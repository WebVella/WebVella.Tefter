namespace WebVella.Tefter.Web.Store.UserDetailsState;

public static partial class UserDetailsStateReducers
{
	[ReducerMethod()]
	public static UserDetailsState SetUserDetailsActionReducer(UserDetailsState state, SetUserDetailsAction action) 
		=> state with { IsBusy = false, User = action.User};
}
