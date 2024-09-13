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
	public static SpaceState ToggleSpaceViewItemSelectionActionReducer(SpaceState state, ToggleSpaceViewItemSelectionAction action)
	{
		if (action.IsSelected)
		{
			var list = state.SelectedDataRows.ToList();
			foreach (var tfId in action.IdList)
			{
				if (list.Contains(tfId)) continue;
				list.Add(tfId);
			}
			return state with
			{
				SelectedDataRows = list
			};
		}
		else
		{
			var list = state.SelectedDataRows.ToList();
			foreach (var tfId in action.IdList)
			{
				if (!list.Contains(tfId)) continue;
				list = state.SelectedDataRows.Where(x => x != tfId).ToList();
			}
			return state with
			{
				SelectedDataRows = list
			};
		}
	}

}
