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
	public static TfAppState SetSpacePagingActionReducer(TfAppState state, SetSpacePagingAction action)
		=> state with
		{
			Page = action.Page,
			PageSize = action.PageSize,
		};
}
