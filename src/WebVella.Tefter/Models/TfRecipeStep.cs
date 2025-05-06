namespace WebVella.Tefter;

public abstract class TfRecipeStepBase
{
	public Guid StepId { get; set; }

	//name of the step
	public string StepName { get; set; }

}

public class TfCreateRoleRecipeStep : TfRecipeStepBase
{
	//name of the step
	public Guid RoleId { get; set; }
	public string RoleName { get; set; }
	public string RoleDescription { get; set; }
}

public class TfCreateMultipleRolesRecipeStep : TfRecipeStepBase
{
	public List<TfCreateRoleRecipeStep> Roles { get; set; }
}

public class TfCreateUserRecipeStep : TfRecipeStepBase
{
	public string UserEmail { get; set; }
	public string UserPassword { get; set; }
	public List<Guid> UserRoles { get; set; }
}

public class TfCreateMultipleUsersRecipeStep : TfRecipeStepBase
{
	public List<TfCreateUserRecipeStep> Users { get; set; }
}

public class TfCreateDataProviderRecipeStep : TfRecipeStepBase
{
	public Type DataProvider { get; set; }
	public string Name { get; set; }
	public List<Guid> Columns { get; set; }
}
