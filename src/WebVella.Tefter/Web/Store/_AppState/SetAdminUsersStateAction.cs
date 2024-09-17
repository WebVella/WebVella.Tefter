namespace WebVella.Tefter.Web.Store;

public record SetAdminUsersStateAction : TfBaseAction
{
	public List<TucUser> AdminUsers { get; }
	public int AdminUsersPage { get; }
	internal SetAdminUsersStateAction(
		FluxorComponent component,
		List<TucUser> adminUsers,
		int adminUsersPage
		)
	{
		StateComponent = component;
		AdminUsers = adminUsers;
		AdminUsersPage = adminUsersPage;
	}
}

public static partial class AppStateReducers
{
	[ReducerMethod()]
	public static TfAppState SetAdminUsersStateActionReducer(TfAppState state, SetAdminUsersStateAction action)
		=> state with { AdminUsers = action.AdminUsers, AdminUsersPage = action.AdminUsersPage };
}