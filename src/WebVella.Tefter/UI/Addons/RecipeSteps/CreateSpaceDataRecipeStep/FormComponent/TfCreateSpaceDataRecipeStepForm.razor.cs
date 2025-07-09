namespace WebVella.Tefter.UI.Addons.RecipeSteps;
public partial class TfCreateSpaceDataRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfCreateSpaceDataRecipeStep Addon { get; set; }
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

