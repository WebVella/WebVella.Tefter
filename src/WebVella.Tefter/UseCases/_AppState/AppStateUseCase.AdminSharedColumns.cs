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
			!(newAppState.Route.FirstNode == RouteDataFirstNode.Admin
			&& newAppState.Route.SecondNode == RouteDataSecondNode.SharedColumns)
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

	internal virtual List<TucSharedColumn> CreateSharedColumn(
		TucSharedColumnForm form)
	{
		if (form.Id == Guid.Empty)
			form = form with { Id = Guid.NewGuid() };


		_tfService.CreateSharedColumn(form.ToModel());

		return GetSharedColumns();
	}

	internal virtual List<TucSharedColumn> UpdateSharedColumn(
		TucSharedColumnForm form)
	{
		_tfService.UpdateSharedColumn(form.ToModel());

		return GetSharedColumns();
	}

	internal virtual List<TucSharedColumn> DeleteSharedColumn(
		Guid columnId)
	{
		try
		{
			_tfService.DeleteSharedColumn(columnId);
			return GetSharedColumns();
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
					if(item.Type == TfDatabaseColumnType.AutoIncrement) continue;
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
