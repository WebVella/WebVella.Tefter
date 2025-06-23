namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<(TfAppState, TfAuxDataState)> InitSpaceDataAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState,
		TfAppState oldAppState,
		TfAuxDataState newAuxDataState,
		TfAuxDataState oldAuxDataState)
	{
		if (newAppState.Space is null)
		{
			newAppState = newAppState with
			{
				SpaceData = null,
				SpaceDataList = new(),
				AllDataProviders = new(),
				SpaceDataData = null,
				SpaceDataPage = 1,
				SpaceDataPageSize = TfConstants.PageSize,
				SpaceDataSearch = null
			};
			return (newAppState, newAuxDataState);
		}
		if (newAppState.Route.SpaceDataId is not null)
		{
			var spaceData = GetSpaceData(newAppState.Route.SpaceDataId.Value);
			if (spaceData is not null)
			{
				newAppState = newAppState with { SpaceData = spaceData };

				//if provider is not found then we init space data as null
				var provider = await GetDataProviderAsync(spaceData.DataProviderId);
				if (provider is not null)
				{
					//Space Data data init
					if (newAppState.Route.HasNode(RouteDataNode.Data, 4))
					{
						TfDataTable viewData = null;
						try
						{
							viewData = GetSpaceDataDataTable(
								spaceDataId: newAppState.SpaceData.Id,
								userFilters: null,
								userSorts: null,
								search: newAppState.Route.Search,
								page: newAppState.Route.Page,
								pageSize: newAppState.Route.PageSize ?? TfConstants.PageSize
							);
						}
						catch { }

						newAppState = newAppState with
						{
							SpaceDataData = viewData,
							SpaceDataPage = viewData?.QueryInfo.Page ?? (newAppState.Route.Page ?? 1),
							SpaceDataPageSize = newAppState.Route.PageSize ?? TfConstants.PageSize,
							SpaceDataSearch = newAppState.Route.Search
						};
					}
				}
			}
		}
		else
		{
			newAppState = newAppState with { SpaceData = null };
		}
		newAppState = newAppState with { AllDataProviders = GetDataProviderList() };

		return (newAppState, newAuxDataState);
	}
	internal virtual TucSpaceData GetSpaceData(
		Guid spaceDataId)
	{
		try
		{
			var spaceData = _tfService.GetSpaceData(spaceDataId);
			if (spaceData is null)
				return null;
			var result = new TucSpaceData(spaceData);

			var allDataProviders = _tfService.GetDataProviders();
			var allDataIdentities = _tfService.GetDataIdentities();
			var allSharedColumns = _tfService.GetSharedColumns();
			GetAllColumns(result, allDataProviders, allDataIdentities,allSharedColumns);

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

	internal virtual List<TucSpaceData> GetSpaceDataList(
		Guid spaceId)
	{
		try
		{
			var spaceDataList = _tfService.GetSpaceDataList(spaceId);
			if (spaceDataList is null)
				return new();

			return spaceDataList
				.Select(x => new TucSpaceData(x))
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

	internal virtual List<TucSpaceData> GetAllSpaceData()
	{
		try
		{
			var spaceDataList = _tfService.GetAllSpaceData();
			if (spaceDataList is null)
				return new();

			return spaceDataList
				.Select(x => new TucSpaceData(x))
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

	internal virtual void DeleteSpaceData(
		Guid dataId)
	{
		_tfService.DeleteSpaceData(dataId);
	}


	internal virtual TucSpaceData CreateSpaceDataWithForm(
		TucSpaceData form)
	{
		TfSpace space = null;
		TfDataProvider dataprovider = null;

		#region << Validate >>

		var valEx = new TfValidationException();
		//args
		if (String.IsNullOrWhiteSpace(form.Name))
			valEx.AddValidationError(nameof(form.Name), "name is required");

		if (form.SpaceId == Guid.Empty)
			valEx.AddValidationError(nameof(form.SpaceId), "space is required");

		if (form.DataProviderId == Guid.Empty)
			valEx.AddValidationError(nameof(form.DataProviderId), "dataprovider is required");

		//Space
		space = _tfService.GetSpace(form.SpaceId);
		if (space is null)
			valEx.AddValidationError(nameof(form.SpaceId), "space is not found");

		//DataProvider
		if (form.DataProviderId != Guid.Empty)
		{
			dataprovider = _tfService.GetDataProvider(form.DataProviderId);
			if (dataprovider is null)
				valEx.AddValidationError(nameof(form.DataProviderId), "data provider is not found");
		}

		valEx.ThrowIfContainsErrors();

		#endregion

		var spaceDataObj = new TfCreateSpaceData()
		{
			Id = Guid.NewGuid(),
			Name = form.Name,
			Filters = new(),//filters will not be added at this point
			Columns = new(), // columns will be added later
			DataProviderId = dataprovider.Id,
			SpaceId = space.Id,
		};

		var spaceData = _tfService.CreateSpaceData(spaceDataObj);

		return new TucSpaceData(spaceData);
	}

	internal virtual TucSpaceData UpdateSpaceDataWithForm(
		TucSpaceData form)
	{
		TfSpace space = null;
		TfSpaceData spaceData = null;
		TfDataProvider dataprovider = null;

		#region << Validate>>

		var valEx = new TfValidationException();

		//args
		if (form.Id == Guid.Empty)
			valEx.AddValidationError(nameof(form.Id), "required");

		if (String.IsNullOrWhiteSpace(form.Name))
			valEx.AddValidationError(nameof(form.Name), "required");

		if (form.SpaceId == Guid.Empty)
			valEx.AddValidationError(nameof(form.SpaceId), "required");

		if (form.DataProviderId == Guid.Empty)
			valEx.AddValidationError(nameof(form.DataProviderId), "required");

		//Space
		space = _tfService.GetSpace(form.SpaceId);
		if (space is null)
			valEx.AddValidationError(nameof(form.SpaceId), "space is not found");

		//DataProvider
		if (form.DataProviderId != Guid.Empty)
		{
			dataprovider = _tfService.GetDataProvider(form.DataProviderId);
			if (dataprovider is null)
				valEx.AddValidationError(nameof(form.DataProviderId), "data provider is not found");
		}

		//SpaceData
		spaceData = _tfService.GetSpaceData(form.Id);
		if (spaceData is null)
			valEx.AddValidationError(nameof(form.Id), "dataset is not found");

		valEx.ThrowIfContainsErrors();

		#endregion

		spaceData.Name = form.Name;
		spaceData.DataProviderId = form.DataProviderId;

		var updatedSpaceData = _tfService.UpdateSpaceData(new TfUpdateSpaceData
		{
			Id = spaceData.Id,
			Name = spaceData.Name,
			DataProviderId = spaceData.DataProviderId,
			Filters = form.Filters.Select(x => TucFilterBase.ToModel(x)).ToList(),
			Columns = form.Columns,
			SortOrders = form.SortOrders.Select(x => x.ToModel()).ToList(),
		});

		//Should commit transaction
		return new TucSpaceData(updatedSpaceData);
	}

	internal virtual TucSpaceData AddSpaceDataColumn(
		Guid spaceDataId,
		TucSpaceDataColumn column)
	{
		if (spaceDataId == Guid.Empty)
			new TfException("spaceDataId is required");

		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
			new TfException("spaceData not found");

		if (column.SourceType == TfAuxDataSourceType.PrimatyDataProvider)
		{
			if (!spaceData.Columns.Any(x => x.ToLowerInvariant() == column.ColumnName.ToLowerInvariant()))
			{
				spaceData.Columns.Add(column.ColumnName);
				var updatedSpaceData = _tfService.UpdateSpaceData(spaceData.ToUpdateModel());
			}
		}
		else if (column.SourceType == TfAuxDataSourceType.AuxDataProvider
		|| column.SourceType == TfAuxDataSourceType.SharedColumn)
		{
			var dataIdentity = spaceData.Identities.FirstOrDefault(x => x.DataIdentity == column.DataIdentity.Name);
			if (dataIdentity is null)
			{
				_tfService.CreateSpaceDataIdentity(new TfSpaceDataIdentity
				{
					Id = Guid.NewGuid(),
					SpaceDataId = spaceDataId,
					Columns = new List<string> { column.SourceColumnName },
					DataIdentity = column.DataIdentity.Name,
				});
			}
			else
			{
				if (dataIdentity.Columns is null)
					dataIdentity.Columns = new();

				if (!dataIdentity.Columns.Any(x => x == column.SourceColumnName))
				{
					dataIdentity.Columns.Add(column.SourceColumnName);
				}
				_tfService.UpdateSpaceDataIdentity(new TfSpaceDataIdentity
				{
					Id = dataIdentity.Id,
					SpaceDataId = dataIdentity.SpaceDataId,
					Columns = dataIdentity.Columns,
					DataIdentity = dataIdentity.DataIdentity,
				});
			}
		}


		return GetSpaceData(spaceDataId);

	}

	internal virtual TucSpaceData RemoveSpaceDataColumn(
		Guid spaceDataId,
		TucSpaceDataColumn column)
	{
		if (spaceDataId == Guid.Empty)
			new TfException("spaceDataId is required");

		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
			new TfException("spaceData not found");

		if (column.SourceType == TfAuxDataSourceType.PrimatyDataProvider)
		{
			if (spaceData.Columns.Any(x => x.ToLowerInvariant() == column.ColumnName.ToLowerInvariant()))
			{
				spaceData.Columns = spaceData.Columns.Where(x => x.ToLowerInvariant() != column.ColumnName.ToLowerInvariant()).ToList();
				var updatedSpaceData = _tfService.UpdateSpaceData(spaceData.ToUpdateModel());
			}
		}
		else if (column.SourceType == TfAuxDataSourceType.AuxDataProvider
		|| column.SourceType == TfAuxDataSourceType.SharedColumn)
		{
			var dataIdentity = spaceData.Identities.FirstOrDefault(x => x.DataIdentity == column.DataIdentity.Name);
			if (dataIdentity is not null)
			{
				if (dataIdentity.Columns is null)
					dataIdentity.Columns = new();

				if (dataIdentity.Columns.Any(x => x == column.SourceColumnName))
				{
					dataIdentity.Columns.Remove(column.SourceColumnName);
				}
				if (dataIdentity.Columns.Count > 0)
				{
					_tfService.UpdateSpaceDataIdentity(new TfSpaceDataIdentity
					{
						Id = dataIdentity.Id,
						SpaceDataId = dataIdentity.SpaceDataId,
						Columns = dataIdentity.Columns,
						DataIdentity = dataIdentity.DataIdentity,
					});
				}
				else
				{
					_tfService.DeleteSpaceDataIdentity(dataIdentity.Id);
				}
			}
		}
		return GetSpaceData(spaceDataId);

	}

	internal virtual TucSpaceData UpdateSpaceDataFilters(
		Guid spaceDataId,
		List<TucFilterBase> filters)
	{
		if (spaceDataId == Guid.Empty)
			throw new TfException("spaceDataId is required");

		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
			throw new TfException("spaceData not found");

		spaceData.Filters = filters;

		var updatedSpaceData = _tfService.UpdateSpaceData(spaceData.ToUpdateModel());

		return new TucSpaceData(updatedSpaceData);

	}

	internal virtual TucSpaceData UpdateSpaceDataSorts(
		Guid spaceDataId,
		List<TucSort> sorts)
	{
		if (spaceDataId == Guid.Empty)
			throw new TfException("spaceDataId is required");

		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
			throw new TfException("spaceData not found");

		spaceData.SortOrders = sorts;

		var updatedSpaceData = _tfService.UpdateSpaceData(spaceData.ToUpdateModel());

		return new TucSpaceData(updatedSpaceData);

	}

	internal virtual void GetAllColumns(TucSpaceData spaceData, ReadOnlyCollection<TfDataProvider> providers,
		List<TfDataIdentity> dataIdentities, List<TfSharedColumn> sharedColumns)
	{
		spaceData.AllColumns = new();
		TfDataProvider provider = providers.Single(x => x.Id == spaceData.DataProviderId);
		foreach (var columnName in spaceData.Columns ?? new List<string>())
		{
			var column = new TucSpaceDataColumn
			{
				Id = null,
				ColumnName = columnName,
				SourceColumnName = columnName,
				SourceCode = null,
				SourceName = null,
				SourceType = TfAuxDataSourceType.NotFound,
				DbType = new TucDatabaseColumnTypeInfo(TfDatabaseColumnType.Text)
			};
			var providerColumn = provider.Columns.FirstOrDefault(x => x.DbName == columnName);

			if (providerColumn is not null)
			{
				column.SourceName = provider.Name;
				column.SourceCode = $"dp{provider.Index}";
				column.SourceType = TfAuxDataSourceType.PrimatyDataProvider;
				column.DbType = new TucDatabaseColumnTypeInfo(providerColumn.DbType);
			}
			spaceData.AllColumns.Add(column);
		}
		foreach (var identity in spaceData.Identities ?? new List<TucSpaceDataIdentity>())
		{
			var identityModel = dataIdentities.FirstOrDefault(x => x.DataIdentity == identity.DataIdentity);
			foreach (var columnName in identity.Columns)
			{
				var column = new TucSpaceDataColumn
				{
					Id = null,
					DataIdentity = new TucDataIdentity(identityModel),
					ColumnName = columnName,
					SourceColumnName = columnName,
					SourceCode = null,
					SourceName = null,
					SourceType = TfAuxDataSourceType.NotFound,
					DbType = new TucDatabaseColumnTypeInfo(TfDatabaseColumnType.Text)
				};
				if (identityModel is null)
				{
					spaceData.AllColumns.Add(column);
					continue;
				}
				if (columnName.StartsWith(TfConstants.TF_SHARED_COLUMN_PREFIX))
				{
					var sharedColumn = sharedColumns.FirstOrDefault(x => x.DbName == columnName);
					if (sharedColumn is not null)
					{
						column.ColumnName = columnName;
						column.SourceName = sharedColumn.DbName;
						column.SourceCode = null;
						column.SourceType = TfAuxDataSourceType.SharedColumn;
						column.DbType = new TucDatabaseColumnTypeInfo(sharedColumn.DbType);
					}
				}
				else
				{

					var providerModel = providers.Single(x => column.SourceColumnName.StartsWith($"dp{x.Index}_"));
					var providerColumn = providerModel.Columns.FirstOrDefault(x => x.DbName == column.SourceColumnName);
					if (providerColumn is not null)
					{
						column.ColumnName = $"{identity.DataIdentity}.{providerColumn.DbName}";
						column.SourceName = providerModel.Name;
						column.SourceCode = $"dp{providerModel.Index}";
						column.SourceType = TfAuxDataSourceType.AuxDataProvider;
						column.DbType = new TucDatabaseColumnTypeInfo(providerColumn.DbType);
					}
				}
				spaceData.AllColumns.Add(column);
			}
		}
	}

	internal virtual List<TucSpaceDataColumn> GetSpaceDataColumnOptions(Guid providerId)
	{
		var result = new List<TucSpaceDataColumn>();
		var auxDataSchemaSM = _tfService.GetDataProviderAuxDataSchema(providerId);
		var auxDataSchema = new TucDataProviderAuxDataSchema(auxDataSchemaSM);
		TfDataProvider provider = _tfService.GetDataProvider(providerId);

		foreach (var providerColumn in provider.Columns)
		{
			var item = new TucSpaceDataColumn
			{
				Id = null,
				DataIdentity = null,
				ColumnName = providerColumn.DbName,
				SourceColumnName = providerColumn.DbName,
				SourceName = provider.Name,
				SourceCode = $"dp{provider.Index}",
				SourceType = TfAuxDataSourceType.PrimatyDataProvider,
				DbType = new TucDatabaseColumnTypeInfo(providerColumn.DbType)
			};
			result.Add(item);
		}

		foreach (var schemaColumn in auxDataSchema.AllColumns)
		{
			var item = new TucSpaceDataColumn
			{
				Id = null,
				DataIdentity = schemaColumn.DataIdentity,
				ColumnName = schemaColumn.DbName,
			};
			if (schemaColumn.SharedColumn is not null)
			{
				item.SourceColumnName = schemaColumn.SharedColumn.DbName;
				item.SourceName = schemaColumn.SharedColumn.DbName;
				item.SourceCode = null;
				item.SourceType = TfAuxDataSourceType.SharedColumn;
				item.DbType = new TucDatabaseColumnTypeInfo(schemaColumn.SharedColumn.DbType.TypeValue);
			}
			else if (schemaColumn.DataProviderColumn is not null)
			{
				item.SourceColumnName = schemaColumn.DataProviderColumn.DbName;
				item.SourceName = schemaColumn.DataProvider.Name;
				item.SourceCode = $"dp{schemaColumn.DataProvider.Index}";
				item.SourceType = TfAuxDataSourceType.AuxDataProvider;
				item.DbType = new TucDatabaseColumnTypeInfo(schemaColumn.DataProviderColumn.DbType);
			}

			result.Add(item);

		}

		return result;
	}

	//Data
	internal virtual TfDataTable GetSpaceDataDataTable(
		Guid spaceDataId,
		List<TucFilterBase> presetFilters = null,
		List<TucSort> presetSorts = null,
		List<TucFilterBase> userFilters = null,
		List<TucSort> userSorts = null,
		string search = null,
		int? page = null,
		int? pageSize = null)
	{
		try
		{
			if (spaceDataId == Guid.Empty)
				throw new TfException("spaceDataId not provided");

			var spaceData = GetSpaceData(spaceDataId);
			if (spaceData is null)
				throw new TfException("Space Dataset is not found");

			List<TfFilterBase> presetFiltersSM = null;
			List<TfSort> presetSortsSM = null;
			List<TfFilterBase> userFiltersSM = null;
			List<TfSort> userSortsSM = null;
			if (presetFilters is not null) presetFiltersSM = presetFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
			if (presetSorts is not null) presetSortsSM = presetSorts.Select(x => x.ToModel()).ToList();
			if (userFilters is not null) userFiltersSM = userFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
			if (userSorts is not null) userSortsSM = userSorts.Select(x => x.ToModel()).ToList();

			return _tfService.QuerySpaceData(
				spaceDataId: spaceDataId,
				presetFilters: presetFiltersSM,
				presetSorts: presetSortsSM,
				userFilters: userFiltersSM,
				userSorts: userSortsSM,
				search: search,
				page: page,
				pageSize: pageSize
			);
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

	internal virtual List<Guid> GetSpaceDataIdList(
		Guid spaceDataId,
		List<TucFilterBase> presetFilters = null,
		List<TucSort> presetSorts = null,
		List<TucFilterBase> userFilters = null,
		List<TucSort> userSorts = null,
		string search = null,
		int? page = null,
		int? pageSize = null)
	{
		try
		{
			if (spaceDataId == Guid.Empty)
				throw new TfException("spaceDataId not provided");

			var spaceData = GetSpaceData(spaceDataId);
			if (spaceData is null)
				throw new TfException("Space Dataset is not found");

			List<TfFilterBase> presetFiltersSM = null;
			List<TfSort> presetSortsSM = null;
			List<TfFilterBase> userFiltersSM = null;
			List<TfSort> userSortsSM = null;
			if (presetFilters is not null) presetFiltersSM = presetFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
			if (presetSorts is not null) presetSortsSM = presetSorts.Select(x => x.ToModel()).ToList();
			if (userFilters is not null) userFiltersSM = userFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
			if (userSorts is not null) userSortsSM = userSorts.Select(x => x.ToModel()).ToList();

			var dt = _tfService.QuerySpaceData(
				spaceDataId: spaceDataId,
				presetFilters: presetFiltersSM,
				presetSorts: presetSortsSM,
				userFilters: userFiltersSM,
				userSorts: userSortsSM,
				search: search,
				page: page,
				pageSize: pageSize,
				noRows: false,
				returnOnlyTfIds: true
			);

			var result = new List<Guid>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				result.Add((Guid)dt.Rows[i][TfConstants.TEFTER_ITEM_ID_PROP_NAME]);
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

	internal virtual TfDataTable SaveDataDataTable(
		TfDataTable dt)
	{
		return _tfService.SaveDataTable(dt);
	}

	internal virtual void DeleteSpaceDataRows(
		Guid spaceDataId,
		List<Guid> tfIdList)
	{
		try
		{
			if (spaceDataId == Guid.Empty)
				throw new TfException("spaceDataId not provided");

			var spaceData = GetSpaceData(spaceDataId);
			if (spaceData is null)
				throw new TfException("Space Dataset is not found");

			var dataProvider = _tfService.GetDataProvider(spaceData.DataProviderId);
			if (dataProvider is null)
				throw new TfException("GetProvider failed");

			foreach (var tfId in tfIdList)
			{
				_tfService.DeleteDataProviderRowByTfId(dataProvider, tfId);
			}
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

	//Data provider
	internal virtual List<TucDataProvider> GetDataProviderList()
	{
		try
		{
			return _tfService.GetDataProviders()
				.Select(x => new TucDataProvider(x))
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

	internal virtual Task<List<string>> GetAllJoinKeysAsync()
	{
		try
		{
			return Task.FromResult(_tfService.GetAllJoinKeyNames());
		}
		catch (Exception)
		{
			return Task.FromResult(new List<string>());
		}
	}
}
