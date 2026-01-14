namespace WebVella.Tefter.UI.Addons;
public partial class TfCreateDatasetRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfCreateDatasetRecipeStep Addon { get; set; }
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

