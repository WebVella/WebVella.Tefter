namespace WebVella.Tefter.UseCases.Recipe;
internal partial class RecipeUseCase
{
	internal ReadOnlyCollection<ITfRecipeAddon> GetRecipes(){ 
		return _metaService.GetRecipes();
	}

	internal ITfRecipeAddon GetRecipe(Guid id){ 
		return _metaService.GetRecipe(id);
	}

	internal async Task<TfRecipeResult> ApplyRecipe(ITfRecipeAddon recipeAddon){ 
		return await _tfService.ApplyRecipeAsync(recipeAddon);
	}
}
