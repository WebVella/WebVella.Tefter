namespace WebVella.Tefter.UseCases.AppState;

internal partial class AppStateUseCase
{
	internal Task<(TfAppState, TfAuxDataState)> InitAdminSharedColumnsAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState,
		TfAppState oldAppState,
		TfAuxDataState newAuxDataState,
		TfAuxDataState oldAuxDataState)
	{
		if (
			!(newAppState.Route.HasNode(RouteDataNode.Admin, 0)
			&& newAppState.Route.HasNode(RouteDataNode.SharedColumns, 1))
			)
		{
			newAppState = newAppState with
			{
				AdminSharedColumns = new(),
				AdminSharedColumnDataTypes = new()
			};
			return Task.FromResult((newAppState, newAuxDataState));
		}
		;


		//SharedColumns
		if (newAppState.AdminSharedColumns.Count == 0
			|| (newAppState.Route.SharedColumnId is not null && !newAppState.AdminSharedColumns.Any(x => x.Id == newAppState.Route.SharedColumnId))
			)
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

		//DataIdentityId
		if (newAppState.Route.SharedColumnId is not null)
		{
			var sharedColumn = GetSharedColumn(newAppState.Route.SharedColumnId.Value);
			newAppState = newAppState with { AdminSharedColumn = sharedColumn };
		}

		return Task.FromResult((newAppState, newAuxDataState));
	}

	internal virtual List<TucSharedColumn> GetSharedColumns()
	{
		var result = new List<TucSharedColumn>();
		try
		{
			var sharedColumns = _tfService.GetSharedColumns();
			foreach (TfSharedColumn item in sharedColumns)
			{
				result.Add(new TucSharedColumn(item));
			}
			return result;
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

	internal virtual TucSharedColumn GetSharedColumn(Guid columnId)
	{
		TucSharedColumn result = null;
		try
		{
			var tfColumn = _tfService.GetSharedColumn(columnId);
			return new TucSharedColumn(tfColumn);
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

	internal virtual TucSharedColumn CreateSharedColumn(
		TucSharedColumnForm form)
	{
		if (form.Id == Guid.Empty)
			form = form with { Id = Guid.NewGuid() };


		_tfService.CreateSharedColumn(form.ToModel());

		return new TucSharedColumn(_tfService.GetSharedColumn(form.Id));
	}

	internal virtual TucSharedColumn UpdateSharedColumn(
		TucSharedColumnForm form)
	{
		_tfService.UpdateSharedColumn(form.ToModel());

		return new TucSharedColumn(_tfService.GetSharedColumn(form.Id));
	}

	internal virtual void DeleteSharedColumn(
		Guid columnId)
	{
		try
		{
			_tfService.DeleteSharedColumn(columnId);
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

	internal virtual List<TucDatabaseColumnTypeInfo> GetDatabaseColumnTypeInfos()
	{
		try
		{
			var result = new List<TucDatabaseColumnTypeInfo>();
			var resultColumnType = _tfService.GetDatabaseColumnTypeInfos();

			if (resultColumnType is not null)
			{
				foreach (DatabaseColumnTypeInfo item in resultColumnType)
				{
					if (item.Type == TfDatabaseColumnType.AutoIncrement) continue;
					result.Add(new TucDatabaseColumnTypeInfo(item));
				}
			}

			return result;
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
}
