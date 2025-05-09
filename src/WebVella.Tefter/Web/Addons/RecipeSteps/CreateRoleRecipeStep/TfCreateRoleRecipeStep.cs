namespace WebVella.Tefter.Web.Addons.RecipeSteps;
public class TfCreateRoleRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("cf3d7b15-0a18-4e19-8dfc-8b588b2cafca");
	public string AddonName { get; init; } = "CreateRoleRecipeStep";
	public string AddonDescription { get; init; } = "creates role recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateRoleRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }
	public async Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateRoleRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateRoleRecipeStepData)addon.Data;
		var result = await tfService.SaveRoleAsync(new TfRole
		{
			Id = step.RoleId == Guid.Empty ? Guid.NewGuid() : step.RoleId,
			IsSystem = false,
			Name = step.Name
		});
	}
}

public class TfCreateRoleRecipeStepData : ITfRecipeStepAddonData
{
	public Guid RoleId { get; set; }
	public string Name { get; set; }
}