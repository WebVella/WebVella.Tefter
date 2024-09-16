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
	public static TfAppState SetSpaceDataReducer(TfAppState state, SetSpaceDataAction action)
		=> state with
		{
			SpaceData = action.SpaceData,
			SpaceDataList = action.SpaceDataList,
		
		};
}
