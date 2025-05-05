namespace WebVella.Tefter;

public class TfRecipeStepResult
{
	public Guid StepId { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsSuccessful { get; set; }

    public List<TfRecipeStepResultError> Errors { get; set; }

}


public class TfRecipeStepResultError
{
    public string Message { get; set; }
    public string StackTrace { get; set; }
}

