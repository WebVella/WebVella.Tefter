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
	public static SpaceState SetSpaceOnlyReducer(SpaceState state, SetSpaceOnlyAction action)
		=> state with
		{
			Space = action.Space,
		};
}
