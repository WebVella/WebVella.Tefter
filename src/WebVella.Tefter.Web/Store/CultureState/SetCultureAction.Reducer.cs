namespace WebVella.Tefter.Web.Store.CultureState;

public static partial class CultureStateReducers
{
	[ReducerMethod()]
	public static CultureState SetCultureActionReducer(CultureState state, SetCultureAction action) 
		=> state with {Culture = action.Culture};
}
