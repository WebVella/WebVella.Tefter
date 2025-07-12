using System.Threading.Tasks;
using WebVella.Tefter.UI.Addons.RecipeSteps;

namespace WebVella.Tefter.UI.Components;
public partial class TucRecipeDetails : TfBaseComponent
{
	[Inject] private ITfRecipeUIService TfRecipeUIService { get; set; } = default!;
	[Parameter] public Guid RecipeId { get; set; }

	private ITfRecipeAddon _recipe;

	private bool _submitting = false;
	private ITfRecipeStepAddon _activeStep;
	private List<ITfRecipeStepAddon> _visibleSteps = new();
	private TfRecipeResult _recipeResult = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var installData = await TfRecipeUIService.GetInstallDataAsync();
		if (installData is not null)
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);

		_recipe = TfRecipeUIService.GetRecipe(RecipeId);
		if (_recipe is null)
			throw new Exception("Recipe Id not found");
		var position = 1;
		foreach (var step in _recipe.Steps)
		{
			if (!step.Instance.Visible) continue;
			step.Instance.Position = position;
			step.Instance.IsFirst = position == 1;
			position++;
			_visibleSteps.Add(step);
		}
		_visibleSteps.Last().Instance.IsLast = true;

		_visibleSteps.Add(new TfResultRecipeStep()
		{
			Instance = new TfRecipeStepInstance
			{
				StepId = Guid.Empty,
				StepMenuTitle = "Result",
				StepContentTitle = "Result",
				Visible = true,
				Position = position,
				IsFirst = false,
				IsLast = false,
			},
			Data = new TfResultRecipeStepData
			{
				Result = null
			}
		});

		if (_visibleSteps.Count > 0)
			_activeStep = _visibleSteps[0];
	}


	private void _toList()
	{
		Navigator.NavigateTo(TfConstants.InstallPage);
	}

	private void _stepBack()
	{
		if (_visibleSteps.Count == 0 || _visibleSteps.First().Instance.StepId == _activeStep.Instance.StepId)
			return;

		var activeIndex = _visibleSteps.FindIndex(x => x.Instance.StepId == _activeStep.Instance.StepId);
		if (activeIndex > 0)
			_activeStep = _visibleSteps[activeIndex - 1];
	}

	private void _stepNext()
	{
		if (_visibleSteps.Count == 0 || _visibleSteps.Last().Instance.StepId == _activeStep.Instance.StepId)
			return;

		var activeIndex = _visibleSteps.FindIndex(x => x.Instance.StepId == _activeStep.Instance.StepId);
		if (activeIndex < _visibleSteps.Count - 1)
			_activeStep = _visibleSteps[activeIndex + 1];
	}

	private async Task _submit()
	{
		if (_submitting) return;
		_submitting = true;
		_recipeResult = null;
		await InvokeAsync(StateHasChanged);
		try
		{
			if (_activeStep is not null && _activeStep.Instance.FormComponent is not null)
			{
				_activeStep.Instance.FormComponent.SubmitForm();
			}
			if (_activeStep.Instance.Errors.Count > 0)
			{
				ToastService.ShowWarning(LOC("Invalid Data!"));
				return;
			}
			if (!_activeStep.Instance.IsLast && _activeStep.Instance.StepId != Guid.Empty)
			{
				_stepNext();
				return;
			}
			_recipeResult = await TfRecipeUIService.ApplyRecipe(_recipe);
			_recipeResult.ApplyResultToSteps(_recipe.Steps);
			var resultStepBase = _visibleSteps.Single(x => x.GetType() == typeof(TfResultRecipeStep));
			var resultStep = (TfResultRecipeStep)resultStepBase;
			((TfResultRecipeStepData)resultStep.Data).Result = _recipeResult;
			_activeStep = resultStep;

		}
		catch (Exception ex)
		{
			if (ex.InnerException is not null)
				ToastService.ShowError(ex.InnerException.Message);
			else
				ToastService.ShowError(ex.Message);
		}
		finally
		{
			_submitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private string _getStepClasses(ITfRecipeStepAddon step)
	{
		var classList = new List<string>();
		if (step.Instance.StepId == _activeStep?.Instance.StepId)
			classList.Add("active");

		else if (step.Instance.Errors.Count > 0)
		{
			classList.Add("error");
		}
		else
		{
			var currentStepIndex = _visibleSteps.FindIndex(x => x.Instance.StepId == step.Instance.StepId);
			var activeStepIndex = _visibleSteps.FindIndex(x => x.Instance.StepId == _activeStep?.Instance.StepId);
			if (currentStepIndex < activeStepIndex)
				classList.Add("completed");
		}
		return String.Join(" ", classList);
	}

	private void _activateStep(ITfRecipeStepAddon step)
	{
		_activeStep = step;
	}

	private void _goToLogin()
	{
		Navigator.NavigateTo(TfConstants.LoginPageUrl);
	}
}



