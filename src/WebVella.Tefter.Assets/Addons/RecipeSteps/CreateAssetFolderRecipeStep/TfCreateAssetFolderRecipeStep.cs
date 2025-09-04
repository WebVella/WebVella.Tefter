
namespace WebVella.Tefter.Assets.Addons;
public class TfCreateAssetFolderRecipeStep : ITfRecipeStepAddon
{
	public Guid AddonId { get; init; } = new Guid("0468a41b-975f-4495-a86b-82e5a280a180");
	public string AddonName { get; init; } = "CreateAssetsFolderRecipeStep";
	public string AddonDescription { get; init; } = "creates assets folder recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateAssetFolderRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }
	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		IAssetsService assetsService = serviceProvider.GetService<IAssetsService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateAssetFolderRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateAssetFolderRecipeStepData)addon.Data;
		var allSharedColumns = assetsService.GetAllSharedColumns();
		var columnName = step.CountSharedColumnName?.Trim();
		var column = allSharedColumns.FirstOrDefault(x=> x.DbName == columnName);
		if(column is null)
			allSharedColumns.FirstOrDefault(x=> x.DbName.ToLowerInvariant() == columnName.ToLowerInvariant());
		if(column is null) throw new Exception($"Shared column not found with the name: '{step.CountSharedColumnName}'");

		var folder = new AssetsFolder
		{
			Id = step.FolderId != Guid.Empty ? step.FolderId : Guid.NewGuid(),
			DataIdentity = column.DataIdentity,
			CountSharedColumnName = column.DbName,
			Name = step.Name,
		};
		assetsService.CreateFolder(folder);
		return Task.CompletedTask;
	}
}

public class TfCreateAssetFolderRecipeStepData : ITfRecipeStepAddonData
{
	public Guid FolderId { get; set; }
	public string Name { get; set; }
	public string CountSharedColumnName { get; set; }

}