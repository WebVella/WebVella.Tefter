namespace WebVella.Tefter;

public class TfRecipeResult
{
	public DateTime StartedOn { get; set; }
	public DateTime? CompletedOn { get; set; }
	public List<TfRecipeStepResult> Steps { get; set; } = [];
	public bool IsCompleted => Steps.All(s => s.IsCompleted);
	public bool IsSuccessful => Steps.All(s => s.IsCompleted && s.IsSuccessful);

	public void ApplyResultToSteps(List<TfRecipeStepBase> steps)
	{
		foreach (var step in steps)
		{
			var result = Steps.FirstOrDefault(x => x.StepId == step.StepId);
			applyResultToStep(result, step);
		}
	}

	private void applyResultToStep(TfRecipeStepResult result, TfRecipeStepBase step)
	{
		step.Errors = new();
		if(result is null) return;
		foreach (var error in result.Errors.Where(x => x.StepId == step.StepId))
		{
			step.Errors.Add(new ValidationError(error.PropName, error.Message));
		}
		if (step is TfGroupRecipeStep)
		{
			foreach (var substep in ((TfGroupRecipeStep)step).Steps)
			{
				var subResult = result.SubSteps.FirstOrDefault(x=> x.StepId == substep.StepId);
				applyResultToStep(subResult, substep);

			}
		}
	}

}





