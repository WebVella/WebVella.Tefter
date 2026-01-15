namespace WebVella.Tefter.UI.Addons;
public partial class TfInfoRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfInfoRecipeStep Addon { get; set; } = null!;
	private TfInfoRecipeStepData _form = null!;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Addon.Data is null)
			_form = new TfInfoRecipeStepData();

		if (Addon.Data!.GetType().FullName != typeof(TfInfoRecipeStepData).FullName)
			throw new Exception("Wrong model data type provided");

		_form = (TfInfoRecipeStepData)Addon.Data;
		base.InitForm(_form);
	}
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

