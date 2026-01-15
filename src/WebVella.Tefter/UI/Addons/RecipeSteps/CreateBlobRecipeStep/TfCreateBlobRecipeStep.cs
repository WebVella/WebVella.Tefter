
namespace WebVella.Tefter.UI.Addons;
public class TfCreateBlobRecipeStep : ITfRecipeStepAddon
{
	public Guid AddonId { get; init; } = new Guid("7c7eb937-46d8-4814-9f1a-d4f3ab770d26");
	public string AddonName { get; init; } = "CreateBlobRecipeStep";
	public string AddonDescription { get; init; } = "creates blob recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateBlobRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }
	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateBlobRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateBlobRecipeStepData)addon.Data;
		var fileLocalPath = step.Assembly.GetFileFromResourceAndUploadLocally(step.EmbeddedResourceName);
		var blobId = step.BlobId == Guid.Empty ? Guid.NewGuid() : step.BlobId;
		tfService.CreateBlob(
			blobId: blobId,
			localPath: fileLocalPath,
			temporary: step.IsTemporary
		);

		//TODO = REVERSE BLOB
		stepResult.StepCreatedBlobs.Add(blobId);

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

public class TfCreateBlobRecipeStepData : ITfRecipeStepAddonData
{
	public Guid BlobId { get; set; }
	public Assembly Assembly { get; set; }
	public string EmbeddedResourceName { get; set; }
	public bool IsTemporary { get; set; } = false;
}