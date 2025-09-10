namespace WebVella.Tefter.UI.Addons.RecipeSteps;
public class TfCreateSpaceDataRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("cef86567-4ec3-4bfc-a28a-f36db708ec5e");
	public string AddonName { get; init; } = "CreateSpaceDataRecipeStep";
	public string AddonDescription { get; init; } = "creates space data recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateSpaceDataRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }
	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateSpaceDataRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateSpaceDataRecipeStepData)addon.Data;
		var dataProvider = tfService.GetDataProvider(step.DataProviderId);
		var dpPrefix = $"dp{dataProvider.Index}_";
		if (step.Filters.Count > 0 || step.SortOrders.Count > 0)
		{
			step.Filters.ForEach(x => x.FixPrefix(dpPrefix));
			step.SortOrders.ForEach(x => x.FixPrefix(dpPrefix));

		}
		List<string> providerColumns = dataProvider.Columns.Where(x => !String.IsNullOrWhiteSpace(x.DbName)
			&& step.Columns.Contains(x.DbName)).Select(x => x.DbName!).ToList();

		var spData = new TfCreateSpaceData
		{
			Id = step.SpaceDataId == Guid.Empty ? Guid.NewGuid() : step.SpaceDataId,
			SpaceId = step.SpaceId,
			DataProviderId = step.DataProviderId,
			Name = step.Name,
			Columns = providerColumns,
			Filters = step.Filters,
			SortOrders = step.SortOrders,
		};
		foreach (var col in step.Columns)
		{
			var colArray = col.Split(".");
			if (colArray.Length != 2) continue;
			if (!dataProvider.Identities.Any(x => x.DataIdentity == colArray[0])) continue;

			var spaceDataIdentity = spData.Identities.FirstOrDefault(x => x.DataIdentity == colArray[0]);
			if (spaceDataIdentity == null)
			{
				spaceDataIdentity = new TfSpaceDataIdentity
				{
					Id = Guid.NewGuid(),
					Columns = new List<string>(),
					DataIdentity = colArray[0],
					SpaceDataId = spData.Id,
				};
				spData.Identities.Add(spaceDataIdentity);
			}
			spaceDataIdentity.Columns.Add(colArray[1]);
		}


		var result = tfService.CreateSpaceData(spData);

		return Task.CompletedTask;
	}
	public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
	{
		return Task.CompletedTask;
	}
}

public class TfCreateSpaceDataRecipeStepData : ITfRecipeStepAddonData
{
	public Guid SpaceDataId { get; set; }
	public Guid DataProviderId { get; set; }
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public List<string> Columns { get; set; } = new();
	public short Position { get; set; } = 100;
	public List<TfFilterBase> Filters { get; set; } = new();
	public List<TfSort> SortOrders { get; set; } = new();
}