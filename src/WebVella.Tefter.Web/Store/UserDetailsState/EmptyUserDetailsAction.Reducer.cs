namespace WebVella.Tefter.Web.Store.UserDetailsState;

public static partial class UserDetailsStateReducers
{
	[ReducerMethod()]
	public static UserDetailsState EmptyUserDetailsActionReducer(UserDetailsState state, EmptyUserDetailsAction action) 
		=> state with { User = null};
}
