using WebVella.Tefter.UseCases.Install;

namespace WebVella.Tefter.Web.Components;
public partial class TfRecipeDetails : TfBaseComponent
{
	[Inject] private InstallUseCase UC { get; set; }
	[Parameter] public Guid RecipeId { get; set; }

	private ITfRecipeAddon _item;

	private int _index = 1;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_item = UC.GetRecipe(RecipeId);
	}


	private void _toList(){ 
		Navigator.NavigateTo(TfConstants.InstallPage);
	}
}