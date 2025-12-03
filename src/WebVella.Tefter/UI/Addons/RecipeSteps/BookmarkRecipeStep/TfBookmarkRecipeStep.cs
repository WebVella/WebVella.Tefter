using WebVella.Tefter.Services;

namespace WebVella.Tefter.UI.Addons.RecipeSteps;
public class TfBookmarkRecipeStep : ITfRecipeStepAddon
{
	//addon
	public Guid AddonId { get; init; } = new Guid("0b48a2d0-4e34-4806-8e4c-0e08c0c805ac");
	public string AddonName { get; init; } = "BookmarkRecipeStep";
	public string AddonDescription { get; init; } = "creates a bookmark";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfBookmarkRecipeStepForm);
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }

	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfBookmarkRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		ITfService tfService = serviceProvider.GetService<ITfService>()!;
		var step = (TfBookmarkRecipeStepData)addon.Data;

		foreach (var bookmark in step.Bookmarks)
		{
			_ = tfService.CreateBookmark(bookmark);
		}


		return Task.CompletedTask;
	}
	public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
	{
		return Task.CompletedTask;
	}
}

public class TfBookmarkRecipeStepData : ITfRecipeStepAddonData
{
	public List<TfBookmark> Bookmarks { get; set; }
}
