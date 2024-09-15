namespace WebVella.Tefter.Web.Store;

public static partial class SpaceStateReducers
{
	/// <summary>
	/// Sets user in state
	/// </summary>
	/// <param name="state"></param>
	/// <param name="action"></param>
	/// <returns></returns>

	[ReducerMethod()]
	public static TfState SetSpaceViewReducer(TfState state, SetSpaceViewAction action)
		=> state with
		{
			SpaceView = action.SpaceView,
			SpaceViewList = action.SpaceViewList,
		
		};
}
