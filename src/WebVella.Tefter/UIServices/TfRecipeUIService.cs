namespace WebVella.Tefter.UIServices;

public partial interface ITfRecipeUIService
{
	Task<TfInstallData?> GetInstallDataAsync();
	Task SaveInstallDataAsync(TfInstallData data);
	ReadOnlyCollection<ITfRecipeAddon> GetRecipes();
	ITfRecipeAddon GetRecipe(Guid id);
	Task<TfRecipeResult> ApplyRecipe(ITfRecipeAddon recipeAddon);
}
public partial class TfRecipeUIService : ITfRecipeUIService
{
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfRecipeUIService> LOC;

	public TfRecipeUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfRecipeUIService>>() ?? default!;
	}

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
