namespace WebVella.Tefter.Web.Store.SpaceState;

public static partial class SpaceStateReducers
{
	/// <summary>
	/// Sets user in state
	/// </summary>
	/// <param name="state"></param>
	/// <param name="action"></param>
	/// <returns></returns>

	[ReducerMethod()]
	public static SpaceState SetSpaceReducer(SpaceState state, SetSpaceAction action)
		=> state with
		{
			IsBusy = action.IsBusy,
			Space = action.Space,
			RouteSpaceId = action.RouteSpaceId,
			RouteSpaceViewId = action.RouteSpaceViewId
		};
}
