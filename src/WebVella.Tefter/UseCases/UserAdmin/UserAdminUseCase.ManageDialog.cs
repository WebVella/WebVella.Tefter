namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	internal async Task InitForManageDialogAsync()
	{
		Form = new TucUserAdminManageForm();
		var rolesResult = await _identityManager.GetRolesAsync();
		if (rolesResult.IsFailed) throw new Exception("GetUserRolesAsync failed");
		if (rolesResult.Value is not null) AllRoles = rolesResult.Value.Select(x => new TucRole(x)).ToList();
	}

}
