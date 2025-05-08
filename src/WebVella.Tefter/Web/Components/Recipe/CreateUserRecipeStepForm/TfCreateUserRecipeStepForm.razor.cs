using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Components;
public partial class TfCreateUserRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfCreateUserRecipeStep Step { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		ComponentId = Step.StepId;
		base.InitForm(Step);
		Step.Component = this;
	}
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			if (Step.Errors.Count > 0)
			{
				foreach (var item in Step.Errors)
				{
					MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
				}
				EditContext.Validate();
				StateHasChanged();
				EditContext.Validate();
			}

		}
	}

	public override void ValidateForm()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();

		if (String.IsNullOrWhiteSpace(Step.Email))
		{
			errors.Add(new ValidationError(nameof(Step.Email), LOC("required")));
		}
		else if (!Step.Email.IsEmail())
		{
			errors.Add(new ValidationError(nameof(Step.Email), LOC("invalid email")));
		}

		if (String.IsNullOrWhiteSpace(Step.Password))
		{
			errors.Add(new ValidationError(nameof(Step.Password), LOC("required")));
		}
		if (String.IsNullOrWhiteSpace(Step.FirstName))
		{
			errors.Add(new ValidationError(nameof(Step.FirstName), LOC("required")));
		}
		if (String.IsNullOrWhiteSpace(Step.LastName))
		{
			errors.Add(new ValidationError(nameof(Step.LastName), LOC("required")));
		}

		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}
		Step.Errors = errors;
		EditContext.Validate();
		StateHasChanged();
		EditContext.Validate();
	}

}

