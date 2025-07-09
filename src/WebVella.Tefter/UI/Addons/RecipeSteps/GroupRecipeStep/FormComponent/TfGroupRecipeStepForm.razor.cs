namespace WebVella.Tefter.UI.Addons.RecipeSteps;
public partial class TfGroupRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfGroupRecipeStep Addon { get; set; } = default!;

	private TfGroupRecipeStepData _form	= default!;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Addon.Data is null)
			_form = new TfGroupRecipeStepData();

		if (Addon.Data!.GetType().FullName != typeof(TfGroupRecipeStepData).FullName)
			throw new Exception("Wrong model data type provided");

		_form = (TfGroupRecipeStepData)Addon.Data;
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
	public override void SubmitForm()
	{
		var errors = new List<ValidationError>();
		foreach (var step in _form.Steps)
		{
			if (step.Instance.FormComponent is not null)
			{
				step.Instance.FormComponent.SubmitForm();
				errors.AddRange(step.Instance.Errors);
			}
		}
		Addon.Instance.Errors = errors;
		StateHasChanged();
	}
}

