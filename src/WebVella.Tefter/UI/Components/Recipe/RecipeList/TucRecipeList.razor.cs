
namespace WebVella.Tefter.UI.Components;
public partial class TucRecipeList : TfBaseComponent
{
	private ReadOnlyCollection<ITfRecipeAddon> _items;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var installData = await TfUIService.GetInstallDataAsync();
		if (installData is not null)
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);

		_items = TfUIService.GetRecipes();
	}

	private void _select(ITfRecipeAddon recipe)
	{
		Navigator.NavigateTo(String.Format(TfConstants.InstallDetailsPage, recipe.AddonId));
	}
}