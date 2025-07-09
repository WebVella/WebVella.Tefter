namespace WebVella.Tefter.UI.Addons.RecipeSteps;
public class TfCreateDataProviderRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("0e67c4b4-07d6-4e4c-b27e-79e4851a2ac8");
	public string AddonName { get; init; } = "CreateDataProviderRecipeStep";
	public string AddonDescription { get; init; } = "creates data provider";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateDataProviderRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }

	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateDataProviderRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateDataProviderRecipeStepData)addon.Data;
		var dataProvider = tfService.CreateDataProvider(new TfCreateDataProvider
		{
			Id = step.DataProviderId == Guid.Empty ? Guid.NewGuid() : step.DataProviderId,
			Index = step.DataProviderIndex,
			Name = step.Name,
			ProviderType = step.Type,
			SettingsJson = step.SettingsJson,
			SynchPrimaryKeyColumns = new List<string>(),
			SynchScheduleEnabled = step.SynchScheduleEnabled,
			SynchScheduleMinutes = step.SynchScheduleMinutes
		});
		var dpPrefix = $"dp{dataProvider.Index}_";

		if (step.SynchPrimaryKeyColumns.Count > 0)
		{
			var updateModel = new TfUpdateDataProvider()
			{
				Id = dataProvider.Id,
				Name = dataProvider.Name,
				SettingsJson = dataProvider.SettingsJson,
				SynchScheduleEnabled = dataProvider.SynchScheduleEnabled,
				SynchScheduleMinutes = dataProvider.SynchScheduleMinutes,
				SynchPrimaryKeyColumns = new()
			};
			foreach (var columnName in step.SynchPrimaryKeyColumns)
			{
				var fixedColumnName = columnName;
				if (!fixedColumnName.StartsWith(dpPrefix))
					fixedColumnName = dpPrefix + fixedColumnName;

				updateModel.SynchPrimaryKeyColumns.Add(fixedColumnName);
			}

			dataProvider = tfService.UpdateDataProvider(updateModel);
		}

		if (step.Columns.Count > 0)
		{
			//Add prefix if needed
			step.Columns.ForEach(x => x.FixPrefix(dpPrefix));
			dataProvider = tfService.CreateBulkDataProviderColumn(dataProvider.Id, step.Columns);
			if (step.TriggerDataSynchronization)
			{
				tfService.CreateSynchronizationTask(dataProvider.Id, new TfSynchronizationPolicy());
			}
		}

		foreach (var joinKey in step.JoinKeys)
		{
			joinKey.FixPrefix(dpPrefix);
			var keyColumns = new List<TfDataProviderColumn>();
			foreach (var columnName in joinKey.Columns)
			{
				var dpColumn = dataProvider.Columns.FirstOrDefault(x => x.DbName == columnName);
				if (dpColumn is null) continue;
				keyColumns.Add(dpColumn);
			}
			var keySM = new TfDataProviderJoinKey
			{
				Id = joinKey.Id,
				DataProviderId = dataProvider.Id,
				DbName = joinKey.DbName,
				Description = joinKey.Description,
				LastModifiedOn = joinKey.LastModifiedOn,
				Version = joinKey.Version,
				Columns = keyColumns
			};
			var result = tfService.CreateDataProviderJoinKey(keySM);
		}

		return Task.CompletedTask;
	}
}

public class TfCreateDataProviderRecipeStepData : ITfRecipeStepAddonData
{ 
	public Guid DataProviderId { get; set; }
	public int DataProviderIndex { get; set; }
	public ITfDataProviderAddon Type { get; set; }
	public string Name { get; set; }
	public List<TfDataProviderColumn> Columns { get; set; } = new();
	public string SettingsJson { get; set; }
	public List<string> SynchPrimaryKeyColumns { get; set; } = new();
	public bool SynchScheduleEnabled { get; set; } = false;
	public short SynchScheduleMinutes { get; set; } = 60;
	public bool TriggerDataSynchronization { get; set; } = false;
	public List<TfRecipeStepDataProviderJoinKey> JoinKeys { get; set; } = new();
}