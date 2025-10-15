namespace WebVella.Tefter.UI.Addons.RecipeSteps;
public class TfResultRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("de21a7f9-3e61-46e3-aeb6-c98184e4b52e");
	public string AddonName { get; init; } = "ResultRecipeStep";
	public string AddonDescription { get; init; } = "creates result recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfResultRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }
	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfResultRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");


		return Task.CompletedTask;
	}
	public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
	{
		return Task.CompletedTask;
	}
}
public class TfResultRecipeStepData : ITfRecipeStepAddonData
{
	public TfRecipeResult Result { get; set; }
}
