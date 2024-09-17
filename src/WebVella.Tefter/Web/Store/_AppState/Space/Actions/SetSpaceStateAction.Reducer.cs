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
	public static TfAppState SetSpaceReducer(TfAppState state, SetSpaceStateAction action)
		=> state with
		{
			Space = action.Space,
			SpaceView = action.SpaceView,
			SpaceViewList = action.SpaceViewList,
			SpaceData = action.SpaceData,
			SpaceDataList = action.SpaceDataList,
		
		};
}
