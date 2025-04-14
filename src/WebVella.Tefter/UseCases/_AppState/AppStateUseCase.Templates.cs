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

	//Processors
	internal virtual List<ITfTemplateProcessorAddon> GetProcessors()
	{
		try
		{
			return _tfService.GetTemplateProcessors().ToList();
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
			return new List<ITfTemplateProcessorAddon>();
		}
	}

	//Templates
	internal virtual List<TucTemplate> GetTemplates(
		TfTemplateResultType? resultType = null,
		string search = null)
	{
		try
		{
			return _tfService.GetTemplates()
				.Where(x => TemplateMatchSearch(x, search, resultType))
				.OrderBy(x => x.Name)
				.Select(x => new TucTemplate(x))
				.ToList();
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;

		}
	}

	internal virtual List<TucTemplate> GetSpaceDataTemplates(
		Guid spaceDataId,
		string search = null)
	{
		try
		{
			var templates = _tfService.GetTemplates();
			var result = new List<TucTemplate>();
			foreach (var item in templates)
			{
				if (!item.IsEnabled)
					continue;

				if (!item.IsSelectable)
					continue;

				if (!item.SpaceDataList.Contains(spaceDataId))
					continue;

				if (!TemplateMatchSearch(item, search, null))
					continue;

				result.Add(new TucTemplate(item));
			}

			return result.OrderBy(x => x.Name).ToList();
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
			return null;
		}
	}

	internal virtual TucTemplate GetTemplate(
		Guid templateId)
	{
		try
		{
			var template = _tfService.GetTemplate(templateId);
			return new TucTemplate(template);
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
	}

	internal virtual TucTemplate CreateTemplate(
		TucManageTemplateModel submit)
	{
		var template = _tfService.CreateTemplate(submit.ToModel());
		return new TucTemplate(template);
	}
	internal virtual TucTemplate UpdateTemplate(
		TucManageTemplateModel submit)
	{
		var template = _tfService.UpdateTemplate(submit.ToModel());
		return new TucTemplate(template);
	}

	internal virtual TucTemplate UpdateTemplateSettings(
		Guid templateId,
		string settingsJson)
	{
		var template = _tfService.GetTemplate(templateId);
		var submit = new TucManageTemplateModel(template);
		submit.SettingsJson = settingsJson;
		var updatedTemplate = _tfService.UpdateTemplate(submit.ToModel());
		return new TucTemplate(updatedTemplate);
	}

	internal virtual void DeleteTemplate(
		Guid templateId)
	{
		_tfService.DeleteTemplate(templateId);
	}

	internal static bool TemplateMatchSearch(
		TfTemplate template,
		string search = null,
		TfTemplateResultType? resultType = null)
	{
		if (resultType is not null && template.ResultType != resultType.Value)
			return false;

		var stringProcessed = search?.Trim().ToLowerInvariant();
		if (String.IsNullOrWhiteSpace(stringProcessed)) return true;
		else if (template.Name.ToLowerInvariant().Contains(stringProcessed))
		{
			return true;
		}
		else if ((template.Description ?? string.Empty).ToLowerInvariant().Contains(stringProcessed))
		{
			return true;
		}

		return false;
	}

	//Space data
	internal virtual List<TfSpaceDataAsOption> GetSpaceDataOptionsForTemplate()
	{
		var result = new List<TfSpaceDataAsOption>();
		var spaceData = _tfService.GetAllSpaceData();
		var spaceDict = _tfService.GetSpacesList().ToDictionary(x => x.Id);
		foreach (var item in spaceData)
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
