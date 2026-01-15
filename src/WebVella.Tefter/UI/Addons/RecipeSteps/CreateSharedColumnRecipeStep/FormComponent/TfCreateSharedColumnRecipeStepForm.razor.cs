namespace WebVella.Tefter.UI.Addons;
public partial class TfCreateSharedColumnRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfCreateSharedColumnRecipeStep Addon { get; set; }
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

