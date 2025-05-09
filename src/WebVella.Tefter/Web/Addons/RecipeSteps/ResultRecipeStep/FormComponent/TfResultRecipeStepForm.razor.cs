namespace WebVella.Tefter.Web.Addons.RecipeSteps;
public partial class TfResultRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfResultRecipeStep Addon { get; set; }

	private TfResultRecipeStepData _form;

	public List<TfRecipeStepResultError> _errors = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Addon.Data is null)
			throw new Exception("Result data must be provided");

		if (Addon.Data.GetType().FullName != typeof(TfResultRecipeStepData).FullName)
			throw new Exception("Wrong model data type provided");

		_form = (TfResultRecipeStepData)Addon.Data;
		base.InitForm(_form);

		if (!_form.Result.IsSuccessful)
		{
			foreach (var stepResult in _form.Result.Steps)
			{
				_errors.AddRange(stepResult.AllErrors);
			}
		}
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

	private async Task _showDetails(string details)
	{
		var dialog = await DialogService.ShowDialogAsync<TfShowTextDialog>(
				details,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthExtraLarge,
					TrapFocus = false
				});

	}
}

