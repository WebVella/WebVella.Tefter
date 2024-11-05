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

	internal List<TucFile> GetFileRepository(string search, int? page = null, int? pageSize = null)
	{
		var result = new List<TucFile>();
		var tfResult = _fileManager.FindAllFiles(
			startsWithPath: TfConstants.AdminFileRepositoryStartPath,
			containsPath: search,
			includeTempFiles: false,
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
			result = tfResult.Value.Select(x => new TucFile(x)).ToList();

		return result;

	}

	internal Result<TucFile> CreateFile(TucFileForm form)
	{
		var bytesResult = _fileManager.GetBytesFromLocalFileSystemPath(form.LocalFilePath);
		if (File.Exists(form.LocalFilePath))
		{
			File.Delete(form.LocalFilePath);
		}
		if (bytesResult.IsFailed) throw new Exception("GetBytesFromLocalFileSystemPath failed");
		var result = _fileManager.CreateFile(
			filePath: $"{TfConstants.AdminFileRepositoryStartPath}/{form.Name}",
			overwrite: false,
			buffer: bytesResult.Value,
			createdBy: form.CreatedBy);
		if (result.IsFailed)
			return Result.Fail(new Error("CreateFile failed").CausedBy(result.Errors));


		return Result.Ok(new TucFile(result.Value));
	}

	internal Result<TucFile> UpdateFile(TucFileForm form)
	{
		var bytesResult = _fileManager.GetBytesFromLocalFileSystemPath(form.LocalFilePath);
		if (File.Exists(form.LocalFilePath))
		{
			File.Delete(form.LocalFilePath);
		}
		if (bytesResult.IsFailed) throw new Exception("GetBytesFromLocalFileSystemPath failed");
		var result = _fileManager.CreateFile(
			filePath: $"{TfConstants.AdminFileRepositoryStartPath}/{form.Name}",
			overwrite: true,
			buffer: bytesResult.Value,
			createdBy: form.CreatedBy);
		if (result.IsFailed)
			return Result.Fail(new Error("CreateFile failed").CausedBy(result.Errors));


		return Result.Ok(new TucFile(result.Value));
	}

	internal Result DeleteFile(TucFile file)
	{
		var result = _fileManager.DeleteFile(file.FilePath);
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
