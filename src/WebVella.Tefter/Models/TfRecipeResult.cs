using WebVella.Tefter.UI.Addons.RecipeSteps;

namespace WebVella.Tefter.Models;

public class TfRecipeResult
{
	public DateTime StartedOn { get; set; }
	public DateTime? CompletedOn { get; set; }
	public List<TfRecipeStepResult> Steps { get; set; } = new();
	public List<Guid> AllCreatedBlogs
	{
		get
		{
			var list = new List<Guid>();
			if (Steps is not null)
			{
				Steps.ForEach(x => list.AddRange(x.AllCreatedBlobs));
			}
			return list;
		}
	}
	public bool IsCompleted => Steps.All(s => s.IsAllCompleted);
	public bool IsSuccessful => Steps.All(s => s.IsAllCompleted && s.IsAllSuccessful);

	public void ApplyResultToSteps(List<ITfRecipeStepAddon> steps)
	{
		foreach (var step in steps)
		{
			var result = Steps.FirstOrDefault(x => x.StepId == step.Instance.StepId);
			applyResultToStep(result, step);
		}
	}

	private void applyResultToStep(TfRecipeStepResult result, ITfRecipeStepAddon step)
	{
		step.Instance.Errors = new();
		if (result is null) return;
		foreach (var error in result.StepErrors.Where(x => x.StepId == step.Instance.StepId))
		{
			step.Instance.Errors.Add(new ValidationError(error.PropName, error.Message));
		}
		if (step is TfGroupRecipeStep && step.Data is TfGroupRecipeStepData)
		{
			foreach (var substep in ((TfGroupRecipeStepData)((TfGroupRecipeStep)step).Data).Steps)
			{
				var subResult = result.SubSteps.FirstOrDefault(x => x.StepId == substep.Instance.StepId);
				applyResultToStep(subResult, substep);

			}
		}
	}

}





