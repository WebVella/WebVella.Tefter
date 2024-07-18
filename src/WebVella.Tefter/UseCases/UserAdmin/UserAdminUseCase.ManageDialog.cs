namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	internal TucUserAdminManageForm Form { get; set; }
	internal List<TucRole> AllRoles { get; set; } = new();

	internal async Task InitForManageDialogAsync()
	{
		Form = new TucUserAdminManageForm();
		var rolesResult = await _identityManager.GetRolesAsync();
		if (rolesResult.IsFailed) throw new Exception("GetUserRolesAsync failed");
		if (rolesResult.Value is not null) AllRoles = rolesResult.Value.Select(x => new TucRole(x)).ToList();
	}

}
