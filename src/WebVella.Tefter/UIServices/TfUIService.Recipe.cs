namespace WebVella.Tefter.UIServices;

public partial interface ITfUIService
{
	Task<TfInstallData?> GetInstallDataAsync();
	Task SaveInstallDataAsync(TfInstallData data);
	ReadOnlyCollection<ITfRecipeAddon> GetRecipes();
	ITfRecipeAddon GetRecipe(Guid id);
	Task<TfRecipeResult> ApplyRecipe(ITfRecipeAddon recipeAddon);
}
public partial class TfUIService : ITfUIService
{
	public async Task<TfInstallData?> GetInstallDataAsync()
	{
		return await _tfService.GetInstallDataAsync();
	}

	public async Task SaveInstallDataAsync(TfInstallData data)
	{
		await _tfService.SaveInstallDataAsync(data);
	}

	public ReadOnlyCollection<ITfRecipeAddon> GetRecipes()
	{
		return _metaService.GetRecipes();
	}

	public ITfRecipeAddon GetRecipe(Guid id)
	{
		return _metaService.GetRecipe(id);
	}

	public async Task<TfRecipeResult> ApplyRecipe(ITfRecipeAddon recipeAddon)
	{
		return await _tfService.ApplyRecipeAsync(recipeAddon);
	}

}
