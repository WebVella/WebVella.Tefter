namespace WebVella.Tefter.UI.Addons.RecipeSteps;
public class TfCreateSpacePageRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("51ffa95a-97fc-4523-af7d-df92a80af416");
	public string AddonName { get; init; } = "CreateSpacePageRecipeStep";
	public string AddonDescription { get; init; } = "creates space page recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateSpacePageRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }

	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateSpacePageRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateSpacePageRecipeStepData)addon.Data;
		_ = tfService.CreateSpacePage(new TfSpacePage
		{
			Id = step.SpacePageId == Guid.Empty ? Guid.NewGuid() : step.SpacePageId,
			SpaceId = step.SpaceId,
			Name = step.Name,
			Description = step.Description,
			Position = step.Position,
			Type = step.Type,
			ComponentType = step.ComponentType.GetType(),
			ComponentId = step.ComponentId,
			ComponentOptionsJson = step.ComponentOptionsJson,
			ChildPages = step.ChildPages,
			FluentIconName = step.FluentIconName
		});

		return Task.CompletedTask;
	}
	public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
	{
		return Task.CompletedTask;
	}
}
public class TfCreateSpacePageRecipeStepData : ITfRecipeStepAddonData
{
	public Guid SpacePageId { get; set; }
	public Guid SpaceId { get; set; }
	public string Name { get; set; } = null!;
	public string? Description { get; set; } = null;
	public short Position { get; set; } = 100;
	public TfSpacePageType Type { get; set; } = TfSpacePageType.Page;
	public ITfSpacePageAddon ComponentType { get; set; } = null!;
	public Guid? ComponentId { get; set; } = null;
	public string ComponentOptionsJson { get; set; } = "{}";
	public List<TfSpacePage> ChildPages { get; set; } = new();
	public string FluentIconName { get; set; } = null!;
}
