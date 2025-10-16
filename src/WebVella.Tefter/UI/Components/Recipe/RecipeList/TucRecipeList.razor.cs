
using WebVella.Tefter.UI.Addons.Recipes;

namespace WebVella.Tefter.UI.Components;
public partial class TucRecipeList : TfBaseComponent
{
	private List<ITfRecipeAddon> _items = null!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var installData = await TfService.GetInstallDataAsync();
		if (installData is not null)
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
		var blankRecipeId = new BlankRecipeAddon().AddonId;
		_items = TfMetaService.GetRecipes().Where(x=> x.AddonId != blankRecipeId).ToList();
	}

	private void _select(ITfRecipeAddon recipe)
	{
		Navigator.NavigateTo(String.Format(TfConstants.InstallRecipeDetailsPage, recipe.AddonId));
	}
}