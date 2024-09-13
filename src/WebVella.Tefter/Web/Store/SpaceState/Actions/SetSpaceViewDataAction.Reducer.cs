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
	public static SpaceState SetSpaceViewDataReducer(SpaceState state, SetSpaceViewDataAction action)
	{
		Console.WriteLine("SetSpaceViewDataReducer");
		return state with
		{
			SpaceViewData = action.SpaceViewData,
		};
	}
}
