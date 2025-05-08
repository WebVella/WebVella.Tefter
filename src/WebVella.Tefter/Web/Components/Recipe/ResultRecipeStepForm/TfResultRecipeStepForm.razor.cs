using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Components;
public partial class TfResultRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfResultRecipeStep Step { get; set; }

	public List<TfRecipeStepResultError> _errors = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(Step);
		ComponentId = Step.StepId;
		Step.Component = this;
		if (!Step.Result.IsSuccessful)
		{
			foreach (var stepResult in Step.Result.Steps)
			{
				_errors.AddRange(stepResult.Errors);
			}
		}
	}
}

