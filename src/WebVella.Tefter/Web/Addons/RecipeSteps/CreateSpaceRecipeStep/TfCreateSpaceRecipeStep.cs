namespace WebVella.Tefter.Web.Addons.RecipeSteps;
public class TfCreateSpaceRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("64615a2e-d0a7-4459-bc2b-48a9ccad3419");
	public string AddonName { get; init; } = "CreateSpaceRecipeStep";
	public string AddonDescription { get; init; } = "creates space recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateSpaceRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }
	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateSpaceRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateSpaceRecipeStepData)addon.Data;
		var allRoles = tfService.GetRoles();
		var stepRoles = allRoles.Where(x => step.Roles.Contains(x.Id)).ToList();
		var result = tfService.CreateSpace(new TfSpace
		{
			Id = step.SpaceId == Guid.Empty ? Guid.NewGuid() : step.SpaceId,
			Name = step.Name,
			Color = (short)(step.Color is null ? TfColor.Emerald500 : step.Color),
			FluentIconName = !String.IsNullOrWhiteSpace(step.FluentIconName) ? step.FluentIconName : "Apps",
			IsPrivate = step.IsPrivate,
			Position = step.Position,
			Roles = stepRoles
		});

		return Task.CompletedTask;
	}
}

public class TfCreateSpaceRecipeStepData : ITfRecipeStepAddonData
{
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public TfColor? Color { get; set; } = TfColor.Emerald500;
	public string FluentIconName { get; set; }
	public bool IsPrivate { get; set; } = false;
	public short Position { get; set; } = 100;
	public List<Guid> Roles { get; set; } = new();
}
