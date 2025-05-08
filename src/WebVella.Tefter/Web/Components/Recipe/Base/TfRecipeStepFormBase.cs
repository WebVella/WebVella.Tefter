using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Components;
public partial class TfRecipeStepFormBase : TfFormBaseComponent
{
	public virtual void ValidateForm(){}
	[Parameter] public Dictionary<Guid,TfRecipeStepInfo> StepInfoDict { get; set; }

}

