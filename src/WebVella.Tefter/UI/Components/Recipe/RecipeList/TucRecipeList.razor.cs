namespace WebVella.Tefter.UI.Components;
public partial class TucRecipeList : TfBaseComponent
{
	private List<ITfOnboardRecipeAddon> _items = null!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var installData = await TfService.GetInstallDataAsync();
		if (installData is not null)
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
		var blankRecipeId = new BlankOnboardRecipeAddon().AddonId;
		_items = TfMetaService.GetOnboardRecipes().Where(x=> x.AddonId != blankRecipeId).ToList();
	}

	private void _select(ITfOnboardRecipeAddon onboardRecipe)
	{
		Navigator.NavigateTo(String.Format(TfConstants.InstallRecipeDetailsPage, onboardRecipe.AddonId));
	}
}