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
		try
		{
			var files = _repositoryManager.GetFiles(
				filenameContains: search,
				page: page,
				pageSize: pageSize
			);

			return files.Select(x => new TucRepositoryFile(x)).ToList();
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
			return result;
		}

	}

	internal virtual TucRepositoryFile CreateFile(TucFileForm form)
	{
		var file = _repositoryManager.CreateFile(
			filename: Path.GetFileName(form.FileName),
			localPath: form.LocalFilePath,
			createdBy: form.CreatedBy);

		return new TucRepositoryFile(file);
	}

	internal virtual TucRepositoryFile UpdateFile(TucFileForm form)
	{
		_repositoryManager.UpdateFile(
			filename: Path.GetFileName(form.FileName),
			localPath: form.LocalFilePath,
			updatedBy: form.CreatedBy);

		var file = _repositoryManager.GetFile(form.FileName);
		return new TucRepositoryFile(file);
	}

	internal virtual void DeleteFile(string fileName)
	{
		try
		{
			_repositoryManager.DeleteFile(fileName);
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
		}
	}


}
