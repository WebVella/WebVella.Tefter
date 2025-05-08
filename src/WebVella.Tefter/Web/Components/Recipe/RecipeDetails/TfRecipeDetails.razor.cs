using System.Threading.Tasks;
using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Components;
public partial class TfRecipeDetails : TfBaseComponent
{
	[Inject] private RecipeUseCase UC { get; set; }
	[Inject] private RecipeUseCase RecipeUC { get; set; }
	[Parameter] public Guid RecipeId { get; set; }

	private ITfRecipeAddon _recipe;

	private bool _submitting = false;
	private TfRecipeStepBase _activeStep;
	private List<TfRecipeStepBase> _visibleSteps = new();
	private Dictionary<Guid, TfRecipeStepInfo> _stepInfoDict = new();
	private TfRecipeResult _recipeResult = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var installData = await RecipeUC.GetInstallDataAsync();
		if (installData is not null)
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);

		_recipe = UC.GetRecipe(RecipeId);
		var position = 1;
		foreach (var step in _recipe.Steps)
		{
			if (!step.Visible) continue;
			_stepInfoDict[step.StepId] = new TfRecipeStepInfo
			{
				Position = position,
				IsFirst = position == 1,
				IsLast = false
			};
			position++;
			_visibleSteps.Add(step);
		}
		_stepInfoDict[_visibleSteps.Last().StepId].IsLast = true;

		_visibleSteps.Add(new TfResultRecipeStep()
		{
			StepId = Guid.Empty,
			Result = null,
			StepMenuTitle = "Result",
			StepContentTitle = "Result",
			Visible = true
		});
		_stepInfoDict[Guid.Empty] = new TfRecipeStepInfo
		{
			Position = position,
			IsFirst = false,
			IsLast = false
		};

		if (_visibleSteps.Count > 0)
			_activeStep = _visibleSteps[0];
	}


	private void _toList()
	{
		Navigator.NavigateTo(TfConstants.InstallPage);
	}

	private void _stepBack()
	{
		if (_visibleSteps.Count == 0 || _visibleSteps.First().StepId == _activeStep.StepId)
			return;

		var activeIndex = _visibleSteps.FindIndex(x => x.StepId == _activeStep.StepId);
		if (activeIndex > 0)
			_activeStep = _visibleSteps[activeIndex - 1];
	}

	private void _stepNext()
	{
		if (_visibleSteps.Count == 0 || _visibleSteps.Last().StepId == _activeStep.StepId)
			return;

		var activeIndex = _visibleSteps.FindIndex(x => x.StepId == _activeStep.StepId);
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
			if (_activeStep is not null && _activeStep.Component is not null)
			{
				_activeStep.Component.ValidateForm();
			}
			if (_activeStep.Errors.Count > 0)
			{
				ToastService.ShowWarning(LOC("Invalid Data!"));
				return;
			}
			if (!_stepInfoDict[_activeStep.StepId].IsLast && _activeStep.StepId != Guid.Empty)
			{
				_stepNext();
				return;
			}
			_recipeResult = await UC.ApplyRecipe(_recipe);
			_recipeResult.ApplyResultToSteps(_recipe.Steps);
			var resultStepBase = _visibleSteps.Single(x=> x.GetType() == typeof(TfResultRecipeStep));
			var resultStep = (TfResultRecipeStep)resultStepBase;
			resultStep.Result = _recipeResult;
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

	private string _getStepClasses(TfRecipeStepBase step)
	{
		var classList = new List<string>();
		if (step.StepId == _activeStep?.StepId)
			classList.Add("active");

		else if (step.Errors.Count > 0)
		{
			classList.Add("error");
		}
		else
		{
			var currentStepIndex = _visibleSteps.FindIndex(x => x.StepId == step.StepId);
			var activeStepIndex = _visibleSteps.FindIndex(x => x.StepId == _activeStep?.StepId);
			if (currentStepIndex < activeStepIndex)
				classList.Add("completed");
		}
		return String.Join(" ", classList);
	}

	private void _activateStep(TfRecipeStepBase step)
	{
		_activeStep = step;
	}

	private void _goToLogin(){ 
		Navigator.NavigateTo(TfConstants.LoginPageUrl);
	}
}



