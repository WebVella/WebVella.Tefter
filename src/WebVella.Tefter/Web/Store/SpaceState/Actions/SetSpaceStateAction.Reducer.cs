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
	public static SpaceState SetSpaceReducer(SpaceState state, SetSpaceStateAction action)
		=> state with
		{
			IsBusy = action.IsBusy,
			Space = action.Space,
			SpaceView = action.SpaceView,
			SpaceViewList = action.SpaceViewList,
			RouteSpaceId = action.RouteSpaceId,
			RouteSpaceViewId = action.RouteSpaceViewId,
			
		};
}
