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
	public static SpaceState SetSpaceViewReducer(SpaceState state, SetSpaceViewAction action)
		=> state with
		{
			SpaceView = action.SpaceView,
			SpaceViewList = action.SpaceViewList,
		
		};
}
