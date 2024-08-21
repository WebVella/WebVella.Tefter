namespace WebVella.Tefter.Web.Store.SpaceState;

public static partial class SpaceStateReducers
{
	[ReducerMethod()]
	public static SpaceState EmptySpaceStateActionReducer(SpaceState state, EmptySpaceStateAction action) 
		=> state with {IsBusy = action.IsBusy};//, Provider = action.Provider};
}
