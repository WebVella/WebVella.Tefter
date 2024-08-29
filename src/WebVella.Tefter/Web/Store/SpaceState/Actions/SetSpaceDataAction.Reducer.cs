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
	public static SpaceState SetSpaceDataReducer(SpaceState state, SetSpaceDataAction action)
		=> state with
		{
			SpaceData = action.SpaceData,
			SpaceDataList = action.SpaceDataList,
		
		};
}
