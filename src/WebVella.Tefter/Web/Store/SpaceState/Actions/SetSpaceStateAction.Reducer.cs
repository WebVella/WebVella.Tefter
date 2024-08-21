namespace WebVella.Tefter.Web.Store.SpaceState;

public static partial class SpaceStateReducers
{
	[ReducerMethod()]
	public static SpaceState SetSpaceStateActionReducer(SpaceState state, 
		SetSpaceStateAction action) 
		=> state with { IsBusy = action.IsBusy }; //, Provider = action.Provider};
}
