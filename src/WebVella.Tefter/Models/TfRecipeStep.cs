namespace WebVella.Tefter;

public abstract class TfRecipeStepBase
{
	public Guid StepId { get; set; }

	//name of the step
	public string StepMenuTitle { get; set; }
	public string StepContentTitle { get; set; }
	public string StepContentDescription { get; set; }
	public bool Visible { get; set; } = true;
	[JsonIgnore]
	public TfRecipeStepFormBase Component { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}

public class TfRecipeStepInfo
{
	public int Position { get; set; }
	public bool IsFirst { get; set; }
	public bool IsLast { get; set; }
}


public class TfResultRecipeStep : TfRecipeStepBase
{
	public TfRecipeResult Result { get; set; }
}

public class TfInfoRecipeStep : TfRecipeStepBase
{
	public string HtmlContent { get; set; }
}

public class TfGroupRecipeStep : TfRecipeStepBase
{
	public List<TfRecipeStepBase> Steps { get; set; } = new();
}

public class TfCreateRoleRecipeStep : TfRecipeStepBase
{
	//name of the step
	public Guid RoleId { get; set; }
	public string Name { get; set; }
}

public class TfCreateUserRecipeStep : TfRecipeStepBase
{
	public Guid UserId { get; set; }
	public string Email { get; set; }
	public string Password { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public List<Guid> Roles { get; set; }

}

public class TfCreateRepositoryFileRecipeStep : TfRecipeStepBase
{
	public string FileName { get; set; }
	public Assembly Assembly { get; set; }
	public string EmbeddedResourceName { get; set; }
}

public class TfCreateDataProviderRecipeStep : TfRecipeStepBase
{
	public Guid DataProviderId { get; set; }
	public ITfDataProviderAddon Type { get; set; }
	public string Name { get; set; }
	public List<TfDataProviderColumn> Columns { get; set; }
	public string SettingsJson { get; set; }
	public bool ShouldSynchronizeData { get; set; } = false;
}
