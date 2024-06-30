namespace WebVella.Tefter.Web.Store.CultureState;

public static partial class CultureStateReducers
{
	[ReducerMethod()]
	public static CultureState InitCultureActionReducer(CultureState state, InitCultureStateAction action)
		=> state with
		{
			Culture = action.Culture,
			UseCase = action.UseCase,
		};
}
