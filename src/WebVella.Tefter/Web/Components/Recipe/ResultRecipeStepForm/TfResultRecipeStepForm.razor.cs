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
		if (!Step.Result.IsSuccessful)
		{
			foreach (var stepResult in Step.Result.Steps)
			{
				_errors.AddRange(stepResult.Errors);
			}
		}
	}
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

