namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal Task<(TfAppState, TfAuxDataState)> InitAdminFileRepositoryAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (
			!(newAppState.Route.FirstNode == RouteDataFirstNode.Admin
			&& newAppState.Route.SecondNode == RouteDataSecondNode.FileRepository)
			)
		{
			newAppState = newAppState with
			{
				AdminFileRepository = new(),
			};
			return Task.FromResult((newAppState, newAuxDataState));
		};


		//FileRepository
		newAppState = newAppState with
		{
			AdminFileRepository = GetFileRepository(
				search: newAppState.Route.Search,
				page: newAppState.Route.Page ?? 1,
				pageSize: newAppState.Route.PageSize ?? TfConstants.PageSize
			)
		};

		return Task.FromResult((newAppState, newAuxDataState));
	}

	internal virtual List<TucRepositoryFile> GetFileRepository(string search, int? page = null, int? pageSize = null)
	{
		var result = new List<TucRepositoryFile>();
		var tfResult = _repositoryManager.GetFiles(
			filenameContains: search,
			page: page,
			pageSize: pageSize
		);
		if (tfResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("FindAllFiles failed").CausedBy(tfResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return result;
		}

		if (tfResult.Value is not null)
			result = tfResult.Value.Select(x => new TucRepositoryFile(x)).ToList();

		return result;

	}

	internal virtual Result<TucRepositoryFile> CreateFile(TucFileForm form)
	{
		var result = _repositoryManager.CreateFile(
			filename: Path.GetFileName(form.FileName),
			localPath: form.LocalFilePath,
			createdBy: form.CreatedBy);
		if (result.IsFailed)
			return Result.Fail(new Error("CreateFile failed").CausedBy(result.Errors));


		return Result.Ok(new TucRepositoryFile(result.Value));
	}

	internal virtual Result<TucRepositoryFile> UpdateFile(TucFileForm form)
	{
		var result = _repositoryManager.UpdateFile(
			filename: Path.GetFileName(form.FileName),
			localPath: form.LocalFilePath,
			updatedBy: form.CreatedBy);
		if (result.IsFailed)
			return Result.Fail(new Error("CreateFile failed").CausedBy(result.Errors));

		var getResult = _repositoryManager.GetFile(Path.GetFileName(form.LocalFilePath));
		if(getResult.IsFailed)
			return Result.Fail(new Error("GetFileName failed").CausedBy(result.Errors));

		return Result.Ok(new TucRepositoryFile(getResult.Value));
	}

	internal virtual Result DeleteFile(string fileName)
	{
		var result = _repositoryManager.DeleteFile(fileName);
		if (result.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("DeleteFile failed").CausedBy(result.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return result;
		}

		return Result.Ok();
	}


}
