namespace WebVella.Tefter.Web.Store.UserState;

public static partial class UserStateReducers
{
	/// <summary>
	/// Sets user in state
	/// </summary>
	/// <param name="state"></param>
	/// <param name="action"></param>
	/// <returns></returns>

	[ReducerMethod()]
	public static UserState SetUserReducer(UserState state, SetUserAction action) 
		=> state with {IsBusy = false, User = action.User};
}
