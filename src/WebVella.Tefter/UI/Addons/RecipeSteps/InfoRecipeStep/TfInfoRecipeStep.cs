namespace WebVella.Tefter.UI.Addons.RecipeSteps;
public class TfInfoRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("9ff1d47b-7436-49d5-be3a-222d0ea7dc76");
	public string AddonName { get; init; } = "InfoRecipeStep";
	public string AddonDescription { get; init; } = "creates info recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfInfoRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }

	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfInfoRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		return Task.CompletedTask;
	}
	public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
	{
		return Task.CompletedTask;
	}
}

public class TfInfoRecipeStepData : ITfRecipeStepAddonData
{
	public string HtmlContent { get; set; }
}
