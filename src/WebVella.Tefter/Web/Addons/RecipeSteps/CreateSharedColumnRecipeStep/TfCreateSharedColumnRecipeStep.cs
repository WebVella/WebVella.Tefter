namespace WebVella.Tefter.Web.Addons.RecipeSteps;
public class TfCreateSharedColumnRecipeStep : ITfRecipeStepAddon
{
	public Guid AddonId { get; init; } = new Guid("1b812109-b4f2-4d74-902c-b8365161a4b7");
	public string AddonName { get; init; } = "CreateSharedColumnRecipeStep";
	public string AddonDescription { get; init; } = "creates shared column recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateSharedColumnRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }
	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateSharedColumnRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");


		var step = (TfCreateSharedColumnRecipeStepData)addon.Data;
		var column = new TfSharedColumn
		{
			Id = step.SharedColumnId == Guid.Empty ? Guid.NewGuid() : step.SharedColumnId,
			DbName = step.DbName,
			DbType = step.ColumnType,
			IncludeInTableSearch = step.IncludeInTableSearch,
			JoinKeyDbName = step.JoinKey
		};
		column.FixPrefix();
		tfService.CreateSharedColumn(column);

		return Task.CompletedTask;
	}
}

public class TfCreateSharedColumnRecipeStepData : ITfRecipeStepAddonData
{
	public Guid SharedColumnId { get; set; }
	public string DbName { get; set; }
	public bool IncludeInTableSearch { get; set; } = false;
	public TfDatabaseColumnType ColumnType { get; set; } = TfDatabaseColumnType.Text;
	public string JoinKey { get; set; }
}