using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Components;
public partial class TfInfoRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfInfoRecipeStep Step { get; set; }
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			ComponentId = Step.StepId;
			Step.Component = this;
		}
	}
	public override void ValidateForm() { }
}

