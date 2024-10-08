namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal Task<TfAppState> InitAdminSharedColumnsAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser, TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (
			!(routeState.FirstNode == RouteDataFirstNode.Admin
			&& routeState.SecondNode == RouteDataSecondNode.SharedColumns)
			)
		{
			newAppState = newAppState with
			{
				AdminSharedColumns = new(),
				AdminSharedColumnDataTypes = new()
			};
			return Task.FromResult(newAppState);
		};


		//SharedColumns
		if (newAppState.AdminSharedColumns.Count == 0)
			newAppState = newAppState with
			{
				AdminSharedColumns = GetSharedColumns()
			};

		//SharedColumns
		if (newAppState.AdminSharedColumnDataTypes.Count == 0)
			newAppState = newAppState with
			{
				AdminSharedColumnDataTypes = GetDatabaseColumnTypeInfos()
			};


		return Task.FromResult(newAppState);
	}

	internal List<TucSharedColumn> GetSharedColumns()
	{
		var result = new List<TucSharedColumn>();
		var tfResult = _sharedColumnsManager.GetSharedColumns();
		if (tfResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSharedColumns failed").CausedBy(tfResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage:"Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return result;
		}

		if (tfResult.Value is not null)
		{
			foreach (TfSharedColumn item in tfResult.Value)
			{
				result.Add(new TucSharedColumn(item));
			}
		}
		return result;

	}

	internal Result<List<TucSharedColumn>> CreateSharedColumn(TucSharedColumnForm form)
	{
		if (form.Id == Guid.Empty)
			form = form with { Id = Guid.NewGuid() };
		var result = _sharedColumnsManager.CreateSharedColumn(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("CreateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(GetSharedColumns());
	}

	internal Result<List<TucSharedColumn>> UpdateSharedColumn(TucSharedColumnForm form)
	{
		var result = _sharedColumnsManager.UpdateSharedColumn(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("UpdateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(GetSharedColumns());
	}

	internal Result<List<TucSharedColumn>> DeleteSharedColumn(Guid columnId)
	{
		var result = _sharedColumnsManager.DeleteSharedColumn(columnId);
		if (result.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetDatabaseColumnTypeInfos failed").CausedBy(result.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage:"Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return result;
		}

		return Result.Ok(GetSharedColumns());
	}

	internal List<TucDatabaseColumnTypeInfo> GetDatabaseColumnTypeInfos()
	{
		var result = new List<TucDatabaseColumnTypeInfo>();
		var resultColumnType = _dataProviderManager.GetDatabaseColumnTypeInfos();
		if (resultColumnType.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetDatabaseColumnTypeInfos failed").CausedBy(resultColumnType.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage:"Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return result;
		}

		if (resultColumnType.Value is not null)
		{
			foreach (DatabaseColumnTypeInfo item in resultColumnType.Value)
			{
				result.Add(new TucDatabaseColumnTypeInfo(item));
			}
		}
		return result;

	}
}
