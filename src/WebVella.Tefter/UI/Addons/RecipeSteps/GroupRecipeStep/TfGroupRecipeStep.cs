namespace WebVella.Tefter.UI.Addons;
public class TfGroupRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("108d824f-f11c-4512-8fba-6ac7acd48143");
	public string AddonName { get; init; } = "GroupRecipeStep";
	public string AddonDescription { get; init; } = "creates group recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfGroupRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }

	public async Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfGroupRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfGroupRecipeStepData)addon.Data;
		foreach (var substep in step.Steps)
		{
			var subStepResult = await tfService.ApplyStep(substep);
			stepResult.SubSteps.Add(subStepResult);
		}
	}
	public async Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
	{
		var step = (TfGroupRecipeStepData)addon.Data;
		ITfMetaService metaService = serviceProvider.GetService<ITfMetaService>();
		foreach (var substep in step.Steps)
		{
			try
			{
				var subStepResult = stepResult?.SubSteps.FirstOrDefault(x => x.StepId == substep.Instance.StepId);
				var stepAddon = metaService.GetRecipeStep(substep.AddonId);
				if (stepAddon is null) return;
				await stepAddon.ReverseStep(
					serviceProvider: serviceProvider,
					stepBase: substep,
					stepResult: subStepResult);
			}
			catch { }
		}
	}
}

public class TfGroupRecipeStepData : ITfRecipeStepAddonData
{
	public List<ITfRecipeStepAddon> Steps { get; set; } = new();
}
