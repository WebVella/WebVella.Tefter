﻿namespace WebVella.Tefter.Web.Store;

public static partial class SpaceStateReducers
{
	/// <summary>
	/// Sets user in state
	/// </summary>
	/// <param name="state"></param>
	/// <param name="action"></param>
	/// <returns></returns>

	[ReducerMethod()]
	public static TfAppState SetSpaceViewMetaReducer(TfAppState state, SetSpaceViewMetaAction action)
		=> state with
		{
			SpaceViewColumns = action.SpaceViewColumns,
		};
}
