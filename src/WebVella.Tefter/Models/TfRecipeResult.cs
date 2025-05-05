namespace WebVella.Tefter;

public class TfRecipeResult
{
	public DateTime StartedOn { get; set; }
	public DateTime? CompletedOn { get; set; }
    public List<TfRecipeStepResult> Steps { get; set; } = [];
    public bool IsCompleted => Steps.All(s => s.IsCompleted);
    public bool IsSuccessful => Steps.All(s => s.IsCompleted && s.IsSuccessful);

}





