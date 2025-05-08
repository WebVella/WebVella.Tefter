using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Components;
public partial class TfCreateRoleRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfCreateRoleRecipeStep Step { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(Step);
	}
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			ComponentId = Step.StepId;
			Step.Component = this;

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

		if (String.IsNullOrWhiteSpace(Step.Name))
		{
			errors.Add(new ValidationError(nameof(Step.Name), LOC("required")));
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

