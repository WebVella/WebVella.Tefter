namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	internal TucUserAdminManageForm Form { get; set; }
	internal IEnumerable<TucRole> AllRoles { get; set; } = Enumerable.Empty<TucRole>();

	internal async Task InitForm()
	{
		Form = new TucUserAdminManageForm();
		var rolesResult = await GetUserRolesAsync();
		if (rolesResult.IsFailed) throw new Exception("GetUserRolesAsync failed");
		if(rolesResult.Value is not null) AllRoles = rolesResult.Value.AsEnumerable();
	}
}
