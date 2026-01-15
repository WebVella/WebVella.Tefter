namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceCreateDialog : TfFormBaseComponent, IDialogContentComponent<TucSpaceCreateDialogContext>
{
	[Parameter] public TucSpaceCreateDialogContext? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private readonly string _error = string.Empty;
	private bool _submitting = false;
	private TfSpace _form = new();
	private List<ITfSpaceRecipeAddon> _recipes = null!;
	private ITfSpaceRecipeAddon? _selectedRecipe = null;
	private ITfRecipeStepAddon? _activeStep;
	private List<ITfRecipeStepAddon> _visibleSteps = new();
	private TfRecipeResult? _recipeResult = null;
	private bool _showControls => String.IsNullOrWhiteSpace(_error) && _activeStep is not null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		Content.Id ??= Guid.NewGuid();
		_recipes = TfMetaService.GetSpaceRecipes().OrderBy(x=> x.SortIndex).ToList();
		
		InitForm(_form);
	}
	
	private void _select(ITfSpaceRecipeAddon recipe)
	{
		_selectedRecipe = recipe;
		if(_selectedRecipe is null) return;
		_visibleSteps.Clear();
		var position = 1;
		foreach (var step in _selectedRecipe.Steps)
		{
			if (step.Instance.Visible)
				_visibleSteps.Add(step);
			position = SetPosition(step, position);
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
	
	private int SetPosition(ITfRecipeStepAddon step, int position)
	{
		if (!step.Instance.Visible) return position;
		step.Instance.Position = position;
		step.Instance.IsFirst = position == 1;

		if (step is TfGroupRecipeStep)
		{
			var group = (TfGroupRecipeStep)step;
			var data = (TfGroupRecipeStepData)group.Data;
			var subPosition = 1;
			foreach (var substep in data.Steps)
			{
				subPosition = SetPosition(substep, subPosition);
			}
		}

		return position + 1;
	}	
	
	private async Task _save()
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
			_recipeResult = await TfService.ApplyRecipeAsync(_selectedRecipe);
			_recipeResult.ApplyResultToSteps(_selectedRecipe.Steps);
			var resultStepBase = _visibleSteps.Single(x => x.GetType() == typeof(TfResultRecipeStep));
			var resultStep = (TfResultRecipeStep)resultStepBase;
			((TfResultRecipeStepData)resultStep.Data).Result = _recipeResult;
			_activeStep = resultStep;
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpaceCreatedEventPayload(TfAuthLayout.GetState().Space!));					

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
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
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
}

public record TucSpaceCreateDialogContext
{
	public Guid? Id { get; set; }
}
