using Microsoft.Extensions.DependencyInjection;

namespace WebVella.Tefter.Extra.Addons;
public class TfCreateIHSpaceStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("AB2BD6A4-BA92-4D55-A18F-BF962F1EBF02");
	public string AddonName { get; init; } = "Create IH Space";
	public string AddonDescription { get; init; } = "creates a IH Process space based on specifications";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateIHSpaceStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }

	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateIHSpaceStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		ITfService tfService = serviceProvider.GetService<ITfService>()!;
	


		return Task.CompletedTask;
	}
	public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
	{
		return Task.CompletedTask;
	}
}

public record TfCreateIHSpaceStepData : ITfRecipeStepAddonData
{
	public string BuildingCode { get; set; }
}
