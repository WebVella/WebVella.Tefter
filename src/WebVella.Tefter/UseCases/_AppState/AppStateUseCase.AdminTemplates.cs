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
			};
			return Task.FromResult((newAppState, newAuxDataState));
		};

		newAppState = newAppState with
		{
			AdminTemplateProcessors = GetProcessors()
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

	internal virtual Result<TucTemplate> CreateTemplate(TucManageTemplateModel submit)
	{
		//var tfResult = _templateService.CreateTemplate();
		//if (tfResult.IsFailed)
		//{
		//	ResultUtils.ProcessServiceResult(
		//		result: Result.Fail(new Error("GetTemplateProcessors failed").CausedBy(tfResult.Errors)),
		//		toastErrorMessage: "Unexpected Error",
		//		toastValidationMessage: "Invalid Data",
		//		notificationErrorTitle: "Unexpected Error",
		//		toastService: _toastService,
		//		messageService: _messageService
		//	);
		//	return result;
		//}
		return null;
	}

}
