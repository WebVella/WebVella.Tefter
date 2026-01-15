namespace WebVella.Tefter.UI.Addons;
public partial class TfCreateSpaceRecipeStepForm : TfRecipeStepFormBase
{
	[Parameter] public TfCreateSpaceRecipeStep Addon { get; set; }
	
	private TfCreateSpaceRecipeStepData _form = null!;
	
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Addon.Data is null)
			_form = new TfCreateSpaceRecipeStepData();

		if (Addon.Data!.GetType().FullName != typeof(TfCreateSpaceRecipeStepData).FullName)
			throw new Exception("Wrong model data type provided");

		_form = (TfCreateSpaceRecipeStepData)Addon.Data;
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

		if (String.IsNullOrWhiteSpace(_form.Name))
		{
			errors.Add(new ValidationError(nameof(_form.Name), LOC("required")));
		}

		if (_form.Color is null)
		{
			errors.Add(new ValidationError(nameof(_form.Color), LOC("required")));
		}
		if (String.IsNullOrWhiteSpace(_form.FluentIconName))
		{
			errors.Add(new ValidationError(nameof(_form.FluentIconName), LOC("required")));
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

