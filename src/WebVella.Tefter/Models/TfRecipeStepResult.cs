namespace WebVella.Tefter;

public class TfRecipeStepResult
{
	public Guid StepId { get; set; }
	public bool IsStepCompleted { get; set; }
	public bool IsStepSuccessful { get; set; }
	public List<Guid> StepCreatedBlobs { get; set; } = new List<Guid>();
	public List<TfRecipeStepResultError> StepErrors { get; set; } = new();
	public List<TfRecipeStepResult> SubSteps { get; set; } = new();
	public List<TfRecipeStepResultError> AllErrors
	{
		get
		{
			var list = new List<TfRecipeStepResultError>();
			list.AddRange(StepErrors);
			if (SubSteps is not null)
			{
				SubSteps.ForEach(x => list.AddRange(x.AllErrors));
			}
			return list;
		}
	}
	public List<Guid> AllCreatedBlobs
	{
		get
		{
			var list = new List<Guid>();
			list.AddRange(StepCreatedBlobs);
			if (SubSteps is not null)
			{
				SubSteps.ForEach(x => list.AddRange(x.AllCreatedBlobs));
			}
			return list;
		}
	}

	public bool IsAllCompleted
	{
		get
		{
			if (!IsStepCompleted) return false;

			if (SubSteps is not null)
			{
				foreach (var substep in SubSteps)
				{
					if(!substep.IsStepCompleted) return false;
				}
			}
			return true;
		}
	}

	public bool IsAllSuccessful
	{
		get
		{
			if (!IsStepSuccessful) return false;

			if (SubSteps is not null)
			{
				foreach (var substep in SubSteps)
				{
					if(!substep.IsAllSuccessful) return false;
				}
			}
			return true;
		}
	}
}


public class TfRecipeStepResultError
{
	public Guid StepId { get; set; }
	public string StepTypeName { get; set; }
	public string StepName { get; set; }
	public string PropName { get; set; }
	public string Message { get; set; }
	public string StackTrace { get; set; }
}

