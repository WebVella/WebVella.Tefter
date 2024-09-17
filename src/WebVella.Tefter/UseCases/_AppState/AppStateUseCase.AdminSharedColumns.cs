namespace WebVella.Tefter.UseCases.AppStart;
internal partial class AppStateUseCase
{
	internal Task<TfAppState> InitAdminSharedColumns(TucUser currentUser, TfRouteState routeState, TfAppState result)
	{
		if (
			!(routeState.FirstNode == RouteDataFirstNode.Admin
			&& routeState.SecondNode == RouteDataSecondNode.SharedColumns)
			)
		{
			result = result with
			{
				AdminSharedColumns = new(),
				AdminSharedColumnDataTypes = new()
			};
			return Task.FromResult(result);
		};


		//SharedColumns
		if (result.AdminSharedColumns.Count == 0)
			result = result with
			{
				AdminSharedColumns = GetSharedColumns()
			};

		//SharedColumns
		if (result.AdminSharedColumnDataTypes.Count == 0)
			result = result with
			{
				AdminSharedColumnDataTypes = GetDatabaseColumnTypeInfos()
			};


		return Task.FromResult(result);
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
