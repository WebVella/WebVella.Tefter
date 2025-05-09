using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Components;
public partial class TfRecipeList : TfBaseComponent
{
	[Inject] private RecipeUseCase UC { get; set; }
	[Inject] private RecipeUseCase RecipeUC { get; set; }

	private ReadOnlyCollection<ITfRecipeAddon> _items;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var installData = await RecipeUC.GetInstallDataAsync();
		if (installData is not null)
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);

		_items = UC.GetRecipes();
	}

	private void _select(ITfRecipeAddon recipe){ 
		Navigator.NavigateTo(String.Format(TfConstants.InstallDetailsPage,recipe.AddonId));
	}
}