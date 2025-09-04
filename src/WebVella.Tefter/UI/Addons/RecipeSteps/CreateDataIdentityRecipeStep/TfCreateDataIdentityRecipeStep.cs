namespace WebVella.Tefter.UI.Addons.RecipeSteps;
public class TfCreateDataIdentityRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("69c488ca-5957-492b-8bcb-30e8dbf7bc09");
	public string AddonName { get; init; } = "CreateDataIdentityRecipeStep";
	public string AddonDescription { get; init; } = "creates data identity recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateDataIdentityRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }
	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateDataIdentityRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateDataIdentityRecipeStepData)addon.Data;
		var result = tfService.CreateDataIdentity(new TfDataIdentity
		{
			Label = step.Label,
			DataIdentity = step.DataIdentity,
		});
		return Task.CompletedTask;
	}
}

public class TfCreateDataIdentityRecipeStepData : ITfRecipeStepAddonData
{
	public string DataIdentity { get; set; }
	public string Label { get; set; }
}