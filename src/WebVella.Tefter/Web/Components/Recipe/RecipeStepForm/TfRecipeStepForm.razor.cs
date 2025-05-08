using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Components;
public partial class TfRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfRecipeStepBase Step { get; set; }
	[Parameter] public bool IsSubstep { get; set; } = false;

	private TfRecipeStepFormBase compRef;
	private string _renderedStepJson = null;

	protected override bool ShouldRender()
	{
		var stepJson = JsonSerializer.Serialize(Step);
		if(_renderedStepJson != stepJson){ 
			_renderedStepJson =stepJson;
			return true;
		}
		return false;
	}
	public override void ValidateForm() { }
}

