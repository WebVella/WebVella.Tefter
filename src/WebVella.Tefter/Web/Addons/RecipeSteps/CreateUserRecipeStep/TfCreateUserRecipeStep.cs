namespace WebVella.Tefter.Web.Addons.RecipeSteps;
public class TfCreateUserRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("6352c931-39cc-4583-9695-d1525a89cec3");
	public string AddonName { get; init; } = "CreateRoleRecipeStep";
	public string AddonDescription { get; init; } = "creates role recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateUserRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }

	public async Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateUserRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateUserRecipeStepData)addon.Data;
		var allRoles = await tfService.GetRolesAsync();
		var allRolesFound = true;
		foreach (var roleId in step.Roles)
		{
			if (!allRoles.Any(x => x.Id == roleId))
			{
				allRolesFound = false;
				break;
			}
		}
		if (!allRolesFound)
		{
			var valEx = new TfValidationException();
			valEx.AddValidationError(nameof(TfCreateUserRecipeStepData.Roles), "role not found");
			throw valEx;
		}
		var stepRoles = allRoles.Where(x => step.Roles.Contains(x.Id)).ToList();
		var result = await tfService.CreateUserAsync(new TfUser
		{
			Id = step.UserId == Guid.Empty ? Guid.NewGuid() : step.UserId,
			CreatedOn = DateTime.Now,
			Email = step.Email,
			Roles = stepRoles.AsReadOnly(),
			Password = step.Password,
			Enabled = true,
			FirstName = step.FirstName,
			LastName = step.LastName,
			Settings = new TfUserSettings()
		});
	}

}
public class TfCreateUserRecipeStepData : ITfRecipeStepAddonData
{
	public Guid UserId { get; set; }
	public string Email { get; set; }
	public string Password { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public List<Guid> Roles { get; set; } = new();
}