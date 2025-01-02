namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal Task<(TfAppState, TfAuxDataState)> InitAdminTemplatesAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (
			!(newAppState.Route.FirstNode == RouteDataFirstNode.Admin
			&& newAppState.Route.SecondNode == RouteDataSecondNode.Templates)
			)
		{
			newAppState = newAppState with
			{
				AdminTemplateProcessors = new(),
				AdminTemplateList = new(),
				AdminTemplateDetails = null
			};
			return Task.FromResult((newAppState, newAuxDataState));
		};

		newAppState = newAppState with
		{
			AdminTemplateProcessors = GetProcessors(),
			AdminTemplateList = GetTemplates(newAppState.Route.TemplateResultType ?? TfTemplateResultType.Email, newAppState.Route.Search),
			AdminTemplateDetails = newAppState.Route.TemplateId.HasValue ? GetTemplate(newAppState.Route.TemplateId.Value) : null
		};

		return Task.FromResult((newAppState, newAuxDataState));
	}

	internal virtual List<ITfTemplateProcessor> GetProcessors()
	{
		var result = new List<ITfTemplateProcessor>();
		var tfResult = _templateService.GetTemplateProcessors();
		if (tfResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetTemplateProcessors failed").CausedBy(tfResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return result;
		}
		return tfResult.Value.ToList();
	}

	internal virtual List<TucTemplate> GetTemplates(TfTemplateResultType? resultType = null, string search = null)
	{
		var tfResult = _templateService.GetTemplates();
		if (tfResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetTemplates failed").CausedBy(tfResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		var result = tfResult.Value.OrderBy(x => x.Name).Select(x => new TucTemplate(x));
		return result.Where(x => TemplateMatchSearch(x, search, resultType)).ToList();
	}

	internal virtual TucTemplate GetTemplate(Guid templateId)
	{
		var tfResult = _templateService.GetTemplate(templateId);
		if (tfResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetTemplate failed").CausedBy(tfResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		return new TucTemplate(tfResult.Value);
	}

	internal virtual Result<TucTemplate> CreateTemplate(TucManageTemplateModel submit)
	{
		var tfResult = _templateService.CreateTemplate(submit.ToModel());
		if (tfResult.IsFailed) return Result.Fail(new Error("CreateTemplate failed").CausedBy(tfResult.Errors));
		return Result.Ok(new TucTemplate(tfResult.Value));
	}
	internal virtual Result<TucTemplate> UpdateTemplate(TucManageTemplateModel submit)
	{
		var tfResult = _templateService.UpdateTemplate(submit.ToModel());
		if (tfResult.IsFailed) return Result.Fail(new Error("UpdateTemplate failed").CausedBy(tfResult.Errors));
		return Result.Ok(new TucTemplate(tfResult.Value));
	}

	internal virtual Result<TucTemplate> UpdateTemplateSettings(Guid templateId, string settingsJson)
	{
		var getResult = _templateService.GetTemplate(templateId);
		if(getResult.IsFailed)
			return Result.Fail(new Error("GetTemplate failed").CausedBy(getResult.Errors));

		var submit = new TucManageTemplateModel(getResult.Value);
		submit.SettingsJson = settingsJson;
		var tfResult = _templateService.UpdateTemplate(submit.ToModel());
		if (tfResult.IsFailed) return Result.Fail(new Error("UpdateTemplate failed").CausedBy(tfResult.Errors));
		return Result.Ok(new TucTemplate(tfResult.Value));
	}

	internal virtual Result DeleteTemplate(Guid templateId)
	{
		var tfResult = _templateService.DeleteTemplate(templateId);
		if (tfResult.IsFailed) return Result.Fail(new Error("DeleteTemplate failed").CausedBy(tfResult.Errors));
		return Result.Ok();
	}

	internal virtual Dictionary<TfTemplateResultType, int> GetTemplateFoundCount(string search)
	{
		var templates = GetTemplates();
		var result = new Dictionary<TfTemplateResultType, int>();
		foreach (var item in Enum.GetValues<TfTemplateResultType>())
		{
			result[item] = 0;
		}
		var stringProcessed = search?.Trim().ToLowerInvariant();
		if (String.IsNullOrWhiteSpace(stringProcessed)) return result;

		var matchTemplates = templates.Where(x => TemplateMatchSearch(x, stringProcessed));
		foreach (var template in matchTemplates)
		{
			result[template.ResultType]++;
		}
		return result;
	}
	internal static bool TemplateMatchSearch(TucTemplate template, string search = null, TfTemplateResultType? resultType = null)
	{
		if (resultType is not null && template.ResultType != resultType.Value) 
			return false;

		var stringProcessed = search?.Trim().ToLowerInvariant();
		if(String.IsNullOrWhiteSpace(stringProcessed)) return true;
		else if (template.Name.ToLowerInvariant().Contains(stringProcessed))
		{
			return true;
		}
		else if (template.Description.ToLowerInvariant().Contains(stringProcessed))
		{
			return true;
		}
		else if (template.UsedColumns.Any(y => y.ToLowerInvariant().Contains(stringProcessed)))
		{
			return true;
		}


		return false;
	}
}
