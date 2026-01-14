namespace WebVella.Tefter.UI.Addons;
public class TfCreateRepositoryFileRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("16945832-2f4e-4268-98de-5c12c9836e9d");
	public string AddonName { get; init; } = "CreateRepositoryFileRecipeStep";
	public string AddonDescription { get; init; } = "creates repository file recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateRepositoryFileRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }
	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateRepositoryFileRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateRepositoryFileRecipeStepData)addon.Data;
		var fileLocalPath = step.Assembly.GetFileFromResourceAndUploadLocally(step.EmbeddedResourceName);
		var repFile = tfService.CreateRepositoryFile(
			filename: step.FileName,
			localPath: fileLocalPath,
			createdBy: null
		);

		//TODO = REVERSE BLOB
		stepResult.StepCreatedBlobs.Add(repFile.Id);
		return Task.CompletedTask;
	}
	public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
	{
		if(stepResult is null || stepResult.StepCreatedBlobs is null) return Task.CompletedTask;

		ITfService tfService = serviceProvider.GetService<ITfService>();
		try
		{
			foreach (var blobId in stepResult.StepCreatedBlobs)
			{
				tfService.DeleteBlob(blobId);
			}
		}
		catch { }
		return Task.CompletedTask;
	}
}

public class TfCreateRepositoryFileRecipeStepData : ITfRecipeStepAddonData
{
	public string FileName { get; set; }
	public Assembly Assembly { get; set; }
	public string EmbeddedResourceName { get; set; }
}