namespace WebVella.Tefter.Web.Addons.RecipeSteps;
public partial class TfCreateRepositoryFileRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfCreateRepositoryFileRecipeStep Addon { get; set; }
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			ComponentId = Addon.Instance.StepId;
			Addon.Instance.FormComponent = this;
		}
	}
	public override void SubmitForm() { }
}

