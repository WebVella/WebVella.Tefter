using WebVella.Tefter.UseCases.Install;

namespace WebVella.Tefter.Web.Components;
public partial class TfRecipeList : TfBaseComponent
{
	[Inject] private InstallUseCase UC { get; set; }

	private ReadOnlyCollection<ITfRecipeAddon> _items;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_items = UC.GetRecipes();
	}

	private void _select(ITfRecipeAddon recipe){ 
		Navigator.NavigateTo(String.Format(TfConstants.InstallDetailsPage,recipe.Id));
	}
}