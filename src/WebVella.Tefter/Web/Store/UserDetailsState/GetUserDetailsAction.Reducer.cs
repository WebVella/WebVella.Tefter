namespace WebVella.Tefter.Web.Store.UserDetailsState;

public static partial class UserDetailsStateReducers
{
	[ReducerMethod()]
	public static UserDetailsState GetUserDetailsActionReducer(UserDetailsState state, GetUserDetailsAction action) 
		=> state with { IsBusy = true};
}
