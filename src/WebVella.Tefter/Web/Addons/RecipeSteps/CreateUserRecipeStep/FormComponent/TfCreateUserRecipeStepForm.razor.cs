using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Addons.RecipeSteps;
public partial class TfCreateUserRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfCreateUserRecipeStep Addon { get; set; }
	private TfCreateUserRecipeStepData _form;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Addon.Data is null)
			_form = new TfCreateUserRecipeStepData();

		if (Addon.Data.GetType().FullName != typeof(TfCreateUserRecipeStepData).FullName)
			throw new Exception("Wrong model data type provided");

		_form = (TfCreateUserRecipeStepData)Addon.Data;
		base.InitForm(_form);
	}
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			ComponentId = Addon.Instance.StepId;
			Addon.Instance.FormComponent = this;
			if (Addon.Instance.Errors.Count > 0)
			{
				foreach (var item in Addon.Instance.Errors)
				{
					MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
				}
				EditContext.Validate();
				StateHasChanged();
				EditContext.Validate();
			}

		}
	}

	public override void SubmitForm()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();

		if (String.IsNullOrWhiteSpace(_form.Email))
		{
			errors.Add(new ValidationError(nameof(_form.Email), LOC("required")));
		}
		else if (!_form.Email.IsEmail())
		{
			errors.Add(new ValidationError(nameof(_form.Email), LOC("invalid email")));
		}

		if (String.IsNullOrWhiteSpace(_form.Password))
		{
			errors.Add(new ValidationError(nameof(_form.Password), LOC("required")));
		}
		if (String.IsNullOrWhiteSpace(_form.FirstName))
		{
			errors.Add(new ValidationError(nameof(_form.FirstName), LOC("required")));
		}
		if (String.IsNullOrWhiteSpace(_form.LastName))
		{
			errors.Add(new ValidationError(nameof(_form.LastName), LOC("required")));
		}

		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}
		Addon.Instance.Errors = errors;
		Addon.Data = _form;
		EditContext.Validate();
		StateHasChanged();
		EditContext.Validate();
	}

}

