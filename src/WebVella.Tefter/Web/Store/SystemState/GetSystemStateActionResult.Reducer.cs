namespace WebVella.Tefter.Web.Store.SystemState;

public static partial class ScreenStateReducers
{
	[ReducerMethod()]
	public static SystemState GetSystemStateActionResultReducer(SystemState state, GetSystemStateActionResult action)
	{
		return state with {
			IsBusy = false,
			Roles = action.Roles,
			DataProviderColumnTypes = action.DataProviderColumnTypes,
		};
	}
}
