namespace WebVella.Tefter.Web.Store.SystemState;

public static partial class ScreenStateReducers
{
	[ReducerMethod()]
	public static SystemState GetSystemStateActionReducer(SystemState state, GetSystemStateAction action)
	{
		return state with
		{
			IsBusy = true
		};
	}
}
