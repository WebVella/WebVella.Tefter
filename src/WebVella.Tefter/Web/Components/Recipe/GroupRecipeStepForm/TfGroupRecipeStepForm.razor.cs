using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Components;
public partial class TfGroupRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfGroupRecipeStep Step { get; set; }

	private List<TfRecipeStepFormBase> _refList = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		ComponentId = Step.StepId;
		Step.Component = this;
	}
	public override void ValidateForm()
	{
		foreach (var step in Step.Steps)
		{
			if(step.Component is not null)
				step.Component.ValidateForm();
		}
	}
}

