﻿namespace WebVella.Tefter.UseCases.AppState;
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

	//Processors
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

	//Templates
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
		var result = tfResult.Value.Where(x => TemplateMatchSearch(x, search, resultType)).OrderBy(x => x.Name).Select(x => new TucTemplate(x));
		return result.ToList();
	}

	internal virtual List<TucTemplate> GetSpaceDataTemplates(Guid spaceDataId, string search = null)
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
		var result = new List<TucTemplate>();
		foreach (var item in tfResult.Value)
		{
			if (!item.IsEnabled) continue;
			if (!item.IsSelectable) continue;
			if (!item.SpaceDataList.Contains(spaceDataId)) continue;
			if (!TemplateMatchSearch(item, search, null)) continue;
			result.Add(new TucTemplate(item));
		}
		return result.OrderBy(x => x.Name).ToList();
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
		if (getResult.IsFailed)
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

	internal static bool TemplateMatchSearch(TfTemplate template, string search = null, TfTemplateResultType? resultType = null)
	{
		if (resultType is not null && template.ResultType != resultType.Value)
			return false;

		var stringProcessed = search?.Trim().ToLowerInvariant();
		if (String.IsNullOrWhiteSpace(stringProcessed)) return true;
		else if (template.Name.ToLowerInvariant().Contains(stringProcessed))
		{
			return true;
		}
		else if (template.Description.ToLowerInvariant().Contains(stringProcessed))
		{
			return true;
		}

		return false;
	}

	//Space data
	internal virtual List<TfSpaceDataAsOption> GetSpaceDataOptionsForTemplate()
	{
		var result = new List<TfSpaceDataAsOption>();
		var spaceResult = _spaceManager.GetSpacesList();
		if (spaceResult.IsFailed) throw new Exception("GetSpacesList failed");
		var spaceDataResult = _spaceManager.GetAllSpaceData();
		if (spaceDataResult.IsFailed) throw new Exception("GetAllSpaceData failed");
		var spaceDict = spaceResult.Value.ToDictionary(x => x.Id);
		foreach (var item in spaceDataResult.Value)
		{
			result.Add(new TfSpaceDataAsOption
			{
				Id = item.Id,
				Name = item.Name,
				SpaceName = spaceDict[item.SpaceId].Name
			});
		}

		result = result.OrderBy(x => x.SpaceName).ThenBy(x => x.Name).ToList();
		return result;
	}

	//Template processing
	//internal virtual ITfTemplatePreviewResult GenerateTemplatePreviewResult(
	//	ITfTemplateProcessor processor,
	//	TucTemplate template,
	//	TucSpace space,
	//	List<Guid> recordIds)
	//{
	//	return processor.GenerateTemplatePreviewResult(
	//		template:template.ToModel(),
	//		tfSpace:space.ToModel(),
	//		tfRecordIds:recordIds,
	//		serviceProvider:_serviceProvider
	//	);
	//}

	//internal virtual ITfTemplateResult ProcessTemplate(
	//	ITfTemplateProcessor processor,
	//	TucTemplate template,
	//	TucSpace space,
	//	List<Guid> recordIds,
	//	ITfTemplatePreviewResult preview)
	//{
	//	return processor.ProcessTemplate(
	//		template:template.ToModel(),
	//		tfSpace:space.ToModel(),
	//		tfRecordIds:recordIds,
	//		preview:preview,
	//		serviceProvider:_serviceProvider
	//	);
	//}
}
