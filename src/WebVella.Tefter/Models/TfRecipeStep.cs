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

public class TfCreateSpaceRecipeStep : TfRecipeStepBase
{
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public TfColor? Color { get; set; } = TfColor.Emerald500;
	public string FluentIconName { get; set; }
	public bool IsPrivate { get; set; } = false;
	public short Position { get; set; } = 100;
	public List<Guid> Roles { get; set; } = new();
}

public class TfCreateSpaceDataRecipeStep : TfRecipeStepBase
{
	public Guid SpaceDataId { get; set; }
	public Guid DataProviderId { get; set; }
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public List<string> Columns { get; set; } = new();
	public short Position { get; set; } = 100;
	public List<TfFilterBase> Filters { get; set; } = new();
	public List<TfSort> SortOrders { get; set; } = new();
}

public class TfCreateSpaceViewRecipeStep : TfRecipeStepBase
{
	public Guid SpaceViewId { get; set; }
	public Guid SpaceId { get; set; }
	public Guid SpaceDataId { get; set; }
	public string Name { get; set; }
	public short Position { get; set; } = 100;
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.DataGrid;
	public List<TfSpaceViewPreset> Presets { get; set; } = new();
	public TucSpaceViewSettings Settings { get; set; } = new();
	public List<TfSpaceViewColumn> Columns { get; set; } = new();
}

public class TfCreateSpacePageRecipeStep : TfRecipeStepBase
{
	public Guid SpacePageId { get; set; }
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public short Position { get; set; } = 100;
	public TfSpacePageType Type { get; set; } = TfSpacePageType.Page;
	public Type ComponentType { get; set; } = typeof(TfSpaceViewSpacePageAddon);
	public Guid? ComponentId { get; set; } = null;
	public string ComponentOptionsJson { get; set; } = "{}";
	public List<TfSpacePage> ChildPages { get; set; } = new();

	public string FluentIconName { get; set; }
}