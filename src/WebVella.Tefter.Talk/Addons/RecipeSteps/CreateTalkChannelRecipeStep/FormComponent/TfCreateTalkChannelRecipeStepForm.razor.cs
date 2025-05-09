namespace WebVella.Tefter.Talk.Addons;
public partial class TfCreateTalkChannelRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfCreateTalkChannelRecipeStep Addon { get; set; }
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

