namespace WebVella.Tefter.Web.Store;

public static partial class UserStateReducers
{
	/// <summary>
	/// Sets user in state
	/// </summary>
	/// <param name="state"></param>
	/// <param name="action"></param>
	/// <returns></returns>

	[ReducerMethod()]
	public static TfState SetUserStateReducer(TfState state, SetUserStateAction action) 
		=> state with {CurrentUser = action.User, CurrentUserSpaces = action.UserSpaces};
}
