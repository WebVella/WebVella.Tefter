namespace WebVella.Tefter.UseCases.Install;
internal partial class InstallUseCase
{
	internal ReadOnlyCollection<ITfRecipeAddon> GetRecipes(){ 
		return _metaService.GetRecipes();
	}

	internal ITfRecipeAddon GetRecipe(Guid id){ 
		return _metaService.GetRecipe(id);
	}
}
