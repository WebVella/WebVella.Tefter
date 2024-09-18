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
	public static TfAppState InitSpaceViewDetailsActionReducer(TfAppState state, InitSpaceViewDetailsAction action)
	{
		return state with
		{
			SpaceViewPage = action.Page,
			SpaceViewPageSize = action.PageSize,
			SpaceViewSearch = action.SearchQuery,
			SpaceViewFilters = action.Filters,
			SpaceViewSorts = action.Sorts,
			SelectedDataRows = action.SelectedDataRows,
			SpaceViewData = action.SpaceViewData,
		};
	}
}
