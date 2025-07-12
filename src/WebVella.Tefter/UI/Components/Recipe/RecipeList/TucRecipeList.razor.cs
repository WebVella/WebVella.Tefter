
namespace WebVella.Tefter.UI.Components;
public partial class TucRecipeList : TfBaseComponent
{
	[Inject] private ITfRecipeUIService TfRecipeUIService { get; set; } = default!;

	private ReadOnlyCollection<ITfRecipeAddon> _items;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var installData = await TfRecipeUIService.GetInstallDataAsync();
		if (installData is not null)
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);

		_items = TfRecipeUIService.GetRecipes();
	}

	private void _select(ITfRecipeAddon recipe)
	{
		Navigator.NavigateTo(String.Format(TfConstants.InstallDetailsPage, recipe.AddonId));
	}
}