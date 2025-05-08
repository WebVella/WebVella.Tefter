namespace WebVella.Tefter;

public class TfRecipeStepResult
{
	public Guid StepId { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsSuccessful { get; set; }

    public List<TfRecipeStepResultError> Errors { get; set; } = new();
    public List<TfRecipeStepResult> SubSteps { get; set; } = new();

}


public class TfRecipeStepResultError
{
    public Guid StepId { get; set; }
    public string StepName { get; set; }
    public string PropName { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
}

