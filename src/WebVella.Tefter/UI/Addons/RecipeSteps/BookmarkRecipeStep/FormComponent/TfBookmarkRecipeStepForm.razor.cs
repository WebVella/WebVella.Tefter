namespace WebVella.Tefter.UI.Addons;
public partial class TfBookmarkRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfInfoRecipeStep Addon { get; set; } = null!;

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

