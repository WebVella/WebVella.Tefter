﻿using DocumentFormat.OpenXml.Wordprocessing;
using System.Diagnostics.Eventing.Reader;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	Task<TfInstallData?> GetInstallDataAsync();
	Task SaveInstallDataAsync(TfInstallData data);
	Task<TfRecipeResult> ApplyRecipeAsync(ITfRecipeAddon recipe);
	Task<TfRecipeStepResult> ApplyStep(ITfRecipeStepAddon step);
}

public partial class TfService : ITfService
{
	public Task<TfInstallData?> GetInstallDataAsync()
	{
		var setting = GetSetting(TfConstants.InstallDataKey);
		if (setting is null)
			return Task.FromResult((TfInstallData?)null);
		TfInstallData? data = JsonSerializer.Deserialize<TfInstallData>(setting.Value);
		return Task.FromResult(data);
	}

	public Task SaveInstallDataAsync(TfInstallData data)
	{
		if (data is null) throw new ArgumentNullException(nameof(data));
		var setting = new TfSetting
		{
			Key = TfConstants.InstallDataKey,
			Value = JsonSerializer.Serialize(data)
		};
		SaveSetting(setting);
		return Task.CompletedTask;
	}

	public async Task<TfRecipeResult> ApplyRecipeAsync(ITfRecipeAddon recipe)
	{
		var result = new TfRecipeResult()
		{
			StartedOn = DateTime.Now,
		};
		try
		{
			if (recipe == null) throw new ArgumentNullException(nameof(recipe));
			using (TfDatabaseTransactionScope scope = _dbService.CreateTransactionScope())
			{
				foreach (var step in recipe.Steps)
				{
					var stepResult = await ApplyStep(step);
					result.Steps.Add(stepResult);
				}
				result.CompletedOn = DateTime.Now;
				if (result.IsSuccessful)
				{
					var installData = new TfInstallData
					{
						RecipeId = recipe.AddonId,
						RecipeTypeFullName = recipe.GetType().FullName,
						AppliedOn = DateTime.Now
					};
					var setting = new TfSetting
					{
						Key = TfConstants.InstallDataKey,
						Value = JsonSerializer.Serialize(installData)
					};
					SaveSetting(setting);
					scope.Complete();
				}
				else
				{
					foreach (var step in recipe.Steps)
					{
						var stepResult = result.Steps.FirstOrDefault(x => x.StepId == step.Instance.StepId);
						await ReverseStep(step, stepResult);
					}
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			foreach (var step in recipe.Steps)
			{
				var stepResult = result.Steps.FirstOrDefault(x => x.StepId == step.Instance.StepId);
				await ReverseStep(step, stepResult);
			}
			throw ProcessException(ex);
		}
	}

	public async Task<TfRecipeStepResult> ApplyStep(ITfRecipeStepAddon step)
	{
		var stepResult = new TfRecipeStepResult()
		{
			StepId = step.Instance.StepId,
			StepErrors = new()
		};
		try
		{
			var stepAddon = _metaService.GetRecipeStep(step.AddonId);
			if (stepAddon is null) throw new Exception($"Step addon with the id {step.AddonId} is not found");
			await stepAddon.ApplyStep(
				serviceProvider: _serviceProvider,
				stepBase: step,
				stepResult: stepResult);

			stepResult.IsStepCompleted = true;
			stepResult.IsStepSuccessful = !stepResult.StepErrors.Any();
		}
		catch (TfValidationException ex)
		{
			stepResult.IsStepCompleted = true;
			stepResult.IsStepSuccessful = false;
			var errors = ex.GetDataAsValidationErrorList();
			if (errors is not null && errors.Count > 0)
			{
				foreach (ValidationError error in errors)
				{
					stepResult.StepErrors.Add(new TfRecipeStepResultError()
					{
						StepId = step.Instance.StepId,
						StepTypeName = step.GetType().Name,
						StepName = step.Instance.StepMenuTitle,
						PropName = String.Empty,
						Message = error.Message,
					});
				}
			}
			else
			{
				stepResult.StepErrors.Add(new TfRecipeStepResultError()
				{
					StepId = step.Instance.StepId,
					StepTypeName = step.GetType().Name,
					StepName = step.Instance.StepMenuTitle,
					Message = ex.Message,
					StackTrace = ex.StackTrace
				});
			}

		}
		catch (Exception ex)
		{
			stepResult.IsStepCompleted = true;
			stepResult.IsStepSuccessful = false;
			stepResult.StepErrors.Add(new TfRecipeStepResultError()
			{
				StepId = step.Instance.StepId,
				StepTypeName = step.GetType().Name,
				StepName = step.Instance.StepMenuTitle,
				Message = ex.Message,
				StackTrace = ex.StackTrace
			});
		}
		return stepResult;
	}

	public async Task ReverseStep(ITfRecipeStepAddon step, TfRecipeStepResult? stepResult)
	{
		try
		{
			var stepAddon = _metaService.GetRecipeStep(step.AddonId);
			if (stepAddon is null) return;
			await stepAddon.ReverseStep(
				serviceProvider: _serviceProvider,
				stepBase: step,
				stepResult: stepResult);
		}
		catch { }
	}
}
