using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Components;
public abstract class TfRecipeStepFormBase : TfFormBaseComponent
{
	public abstract void ValidateForm();
	[Parameter] public Dictionary<Guid,TfRecipeStepInfo> StepInfoDict { get; set; }

}

