namespace WebVella.Tefter.UI.Addons.RecipeSteps;

public class TfCreateSpaceViewRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("78c55814-dd22-4462-a80c-decdaec1b9b1");
	public string AddonName { get; init; } = "CreateSpaceViewRecipeStep";
	public string AddonDescription { get; init; } = "creates role recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateSpaceViewRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }

	public async Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon,
		TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateSpaceViewRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateSpaceViewRecipeStepData)addon.Data;
		var spaceData = tfService!.GetDataset(step.SpaceDataId);
		if (spaceData is null)
			throw new Exception("Space data not found");
		var dataProvider = tfService.GetDataProvider(spaceData.DataProviderId);
		var dpPrefix = $"dp{dataProvider.Index}_";

		foreach (var preset in step.Presets)
		{
			preset.Filters.ForEach(x => x.FixPrefix(dpPrefix));
			preset.SortOrders.ForEach(x => x.FixPrefix(dpPrefix));
		}

		_ = tfService.CreateSpaceView(new TfSpaceView
		{
			Id = step.SpaceViewId == Guid.Empty ? Guid.NewGuid() : step.SpaceViewId,
			Name = step.Name,
			DatasetId = step.SpaceDataId,
			Presets = step.Presets,
			SettingsJson = JsonSerializer.Serialize(step.Settings),
		});
		;
		foreach (var column in step.Columns)
		{
			await tfService.CreateSpaceViewColumn(column);
		}
	}

	public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
	{
		return Task.CompletedTask;
	}
}

public class TfCreateSpaceViewRecipeStepData : ITfRecipeStepAddonData
{
	public Guid SpaceViewId { get; set; }
	public Guid SpaceId { get; set; }
	public Guid SpaceDataId { get; set; }
	public string Name { get; set; } = string.Empty;
	public short Position { get; set; } = 100;
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.DataGrid;
	public List<TfSpaceViewPreset> Presets { get; set; } = new();
	public TfSpaceViewSettings Settings { get; set; } = new();
	public List<TfSpaceViewColumn> Columns { get; set; } = new();
}