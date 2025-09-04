using System.Reflection;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Addons.RecipeSteps;
public class TfCreateExcelFileTemplateRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("68593034-4f1e-4e3e-a703-8418171a4595");
	public string AddonName { get; init; } = "CreateRoleRecipeStep";
	public string AddonDescription { get; init; } = "creates role recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateExcelFileTemplateRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }

	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateExcelFileTemplateRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateExcelFileTemplateRecipeStepData)addon.Data;
		var result = tfService.CreateTemplate(new TfManageTemplateModel
		{
			Id = step.TemplateId == Guid.Empty ? Guid.NewGuid() : step.TemplateId,
			Name = step.Name,
			FluentIconName = step.FluentIconName,
			ContentProcessorType = step.ContentProcessorType.GetType(),
			Description = step.Description,
			IsEnabled = step.IsEnabled,
			IsSelectable = step.IsSelectable,
			SettingsJson = step.SettingsJson,
			SpaceDataList = step.SpaceDataList,
			UserId = null
		});

		return Task.CompletedTask;
	}
	public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
	{
		return Task.CompletedTask;
	}
}

public class TfCreateExcelFileTemplateRecipeStepData : ITfRecipeStepAddonData
{
	public Guid TemplateId { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string FluentIconName { get; set; }
	public ITfTemplateProcessorAddon ContentProcessorType { get; set; }
	public bool IsEnabled { get; set; } = true;
	public bool IsSelectable { get; set; } = true;
	public string SettingsJson { get; set; } = "{}";
	public List<Guid> SpaceDataList { get; set; } = new();
	/// <summary>
	/// will be used if an uploaded file is needed
	/// </summary>
	public string EmbeddedResourceName { get; set; }
	public string FileName { get; set; }
	public Assembly Assembly { get; set; }
}