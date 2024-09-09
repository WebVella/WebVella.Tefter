namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	private readonly IIdentityManager _identityManager;
	private readonly ITfSpaceManager _spaceManager;
	private readonly ITfDataProviderManager _dataProviderManager;
	private readonly NavigationManager _navigationManager;
	private readonly IDispatcher _dispatcher;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IStringLocalizer<SpaceUseCase> _loc;

	public SpaceUseCase(
			IIdentityManager identityManager,
			ITfSpaceManager spaceManager,
			ITfDataProviderManager dataProviderManager,
			NavigationManager navigationManager,
			IDispatcher dispatcher,
			IToastService toastService,
			IMessageService messageService,
			IStringLocalizer<SpaceUseCase> loc
			)
	{
		_identityManager = identityManager;
		_spaceManager = spaceManager;
		_dataProviderManager = dataProviderManager;
		_navigationManager = navigationManager;
		_dispatcher = dispatcher;
		_toastService = toastService;
		_messageService = messageService;
		_loc = loc;
	}
	internal bool IsBusy { get; set; } = true;
	internal List<TucDataProvider> AllDataProviders { get; set; } = new();

	internal List<TucSpaceViewColumn> ViewColumns = new();

	internal async Task Init(Type type, Guid? spaceId = null)
	{
		if (type == typeof(TfSpaceStateManager)) await InitForState();
		else if (type == typeof(TfSpaceManageDialog)) await InitSpaceManageDialog();
		else if (type == typeof(TfSpaceDataManage)) await InitSpaceDataManage();
		else if (type == typeof(TfSpaceDataViews)) await InitSpaceDataManageViews();
		else if (type == typeof(TfSpaceViewDetails)) await InitSpaceViewDetails();
		else if (type == typeof(TfSpaceViewManage)) await InitSpaceViewManage();
		else if (type == typeof(TfSpaceViewManageDialog)) await InitSpaceViewManageDialog(spaceId.Value);
		else if (type == typeof(TfSpaceDataManageDialog)) await InitSpaceDataManageDialog(spaceId.Value);
		else if (type == typeof(TfSearchSpaceDialog)) await InitForSearchSpace();
		else if (type == typeof(TfSpaceDataFilterManageDialog)) await InitSpaceDataFilterManageDialog();
		else if (type == typeof(TfSpaceViewColumnManageDialog)) await InitSpaceViewColumnManage();
		else if (type == typeof(TfSpaceDetails)) { }
		else throw new Exception($"Type: {type.Name} not supported in SpaceUseCase");
	}

	//Space
	internal Result<TucSpace> CreateSpaceWithForm(TucSpace space)
	{
		var result = _spaceManager.CreateSpace(space.ToModel());
		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceWithFormAsync failed").CausedBy(result.Errors));
		return Result.Ok(new TucSpace(result.Value));
	}

	internal Result<TucSpace> UpdateSpaceWithForm(TucSpace space)
	{
		var result = _spaceManager.UpdateSpace(space.ToModel());
		if (result.IsFailed) return Result.Fail(new Error("UpdateSpaceWithForm failed").CausedBy(result.Errors));
		return Result.Ok(new TucSpace(result.Value));
	}

	internal TucSpace GetSpace(Guid spaceId)
	{
		var serviceResult = _spaceManager.GetSpace(spaceId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpace failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return null;

		return new TucSpace(serviceResult.Value);
	}

	//Space data
	internal TucSpaceData GetSpaceData(Guid spaceDataId)
	{
		var serviceResult = _spaceManager.GetSpaceData(spaceDataId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceData failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return null;

		return new TucSpaceData(serviceResult.Value);
	}

	internal List<TucSpaceData> GetSpaceDataList(Guid spaceId)
	{
		var serviceResult = _spaceManager.GetSpaceDataList(spaceId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceDataList failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucSpaceData(x)).ToList();
	}

	internal Result<TucSpaceData> CreateSpaceDataWithForm(TucSpaceData form)
	{
		TfSpace space = null;
		TfDataProvider dataprovider = null;
		#region << Validate>>
		var validationErrors = new List<ValidationError>();
		//args
		if (String.IsNullOrWhiteSpace(form.Name)) validationErrors.Add(new ValidationError(nameof(form.Name), "name is required"));
		if (form.SpaceId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(form.SpaceId), "space is required"));
		if (form.DataProviderId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(form.DataProviderId), "dataprovider is required"));

		//Space
		var spaceResult = _spaceManager.GetSpace(form.SpaceId);
		if (spaceResult.IsFailed) return Result.Fail(new Error("GetSpace failed").CausedBy(spaceResult.Errors));
		if (spaceResult.Value is null) validationErrors.Add(new ValidationError(nameof(form.SpaceId), "space is not found"));
		space = spaceResult.Value;

		//DataProvider
		if (form.DataProviderId != Guid.Empty)
		{
			var providerResult = _dataProviderManager.GetProvider(form.DataProviderId);
			if (providerResult.IsFailed) return Result.Fail(new Error("GetProvider failed").CausedBy(providerResult.Errors));
			if (providerResult.Value is null) validationErrors.Add(new ValidationError(nameof(form.DataProviderId), "data provider is not found"));
			dataprovider = providerResult.Value;
		}

		if (validationErrors.Count > 0)
			return Result.Fail(validationErrors);

		#endregion

		var spaceDataObj = new TfSpaceData()
		{
			Id = Guid.NewGuid(),
			Name = form.Name,
			Filters = new(),//filters will not be added at this point
			Columns = new(), // columns will be added later
			DataProviderId = dataprovider.Id,
			SpaceId = space.Id,
			Position = 1 //position is overrided in the creation
		};

		var tfResult = _spaceManager.CreateSpaceData(spaceDataObj);
		if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceData failed").CausedBy(tfResult.Errors));
		if (tfResult.Value is null) return Result.Fail("CreateSpaceData failed to return value");

		//Should commit transaction
		return Result.Ok(new TucSpaceData(tfResult.Value));
	}

	internal Result<TucSpaceData> UpdateSpaceDataWithForm(TucSpaceData form)
	{
		TfSpace space = null;
		TfSpaceData spaceData = null;
		TfDataProvider dataprovider = null;
		#region << Validate>>
		var validationErrors = new List<ValidationError>();
		//args
		if (form.Id == Guid.Empty) validationErrors.Add(new ValidationError(nameof(form.Id), "required"));
		if (String.IsNullOrWhiteSpace(form.Name)) validationErrors.Add(new ValidationError(nameof(form.Name), "required"));
		if (form.SpaceId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(form.SpaceId), "required"));
		if (form.DataProviderId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(form.DataProviderId), "required"));

		//Space
		var spaceResult = _spaceManager.GetSpace(form.SpaceId);
		if (spaceResult.IsFailed) return Result.Fail(new Error("GetSpace failed").CausedBy(spaceResult.Errors));
		if (spaceResult.Value is null) validationErrors.Add(new ValidationError(nameof(form.SpaceId), "space is not found"));
		space = spaceResult.Value;

		//DataProvider
		if (form.DataProviderId != Guid.Empty)
		{
			var providerResult = _dataProviderManager.GetProvider(form.DataProviderId);
			if (providerResult.IsFailed) return Result.Fail(new Error("GetProvider failed").CausedBy(providerResult.Errors));
			if (providerResult.Value is null) validationErrors.Add(new ValidationError(nameof(form.DataProviderId), "data provider is not found"));
			dataprovider = providerResult.Value;
		}

		//SpaceData
		var spaceDataResult = _spaceManager.GetSpaceData(form.Id);
		if (spaceDataResult.IsFailed) return Result.Fail(new Error("GetSpaceData failed").CausedBy(spaceDataResult.Errors));
		if (spaceDataResult.Value is null) validationErrors.Add(new ValidationError(nameof(form.Id), "dataset is not found"));
		spaceData = spaceDataResult.Value;



		if (validationErrors.Count > 0)
			return Result.Fail(validationErrors);

		#endregion
		spaceData.Name = form.Name;
		spaceData.DataProviderId = form.DataProviderId;

		var tfResult = _spaceManager.UpdateSpaceData(spaceData);
		if (tfResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(tfResult.Errors));
		if (tfResult.Value is null) return Result.Fail("UpdateSpaceData failed to return value");

		//Should commit transaction
		return Result.Ok(new TucSpaceData(tfResult.Value));
	}

	internal Result<TucSpaceData> AddColumnToSpaceData(Guid spaceDataId, string columnDbName)
	{
		if (spaceDataId == Guid.Empty) return Result.Fail("spaceDataId is required");
		if (String.IsNullOrWhiteSpace(columnDbName)) return Result.Fail("columnDbName is required");
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) return Result.Fail("spaceData not found");
		if (spaceData.Columns.Contains(columnDbName)) return Result.Ok(spaceData);

		spaceData.Columns.Add(columnDbName);
		var updateResult = _spaceManager.UpdateSpaceData(spaceData.ToModel());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(updateResult.Errors));

		return Result.Ok(new TucSpaceData(updateResult.Value));

	}

	internal Result<TucSpaceData> RemoveColumnFromSpaceData(Guid spaceDataId, string columnDbName)
	{
		if (spaceDataId == Guid.Empty) return Result.Fail("spaceDataId is required");
		if (String.IsNullOrWhiteSpace(columnDbName)) return Result.Fail("columnDbName is required");
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) return Result.Fail("spaceData not found");
		if (!spaceData.Columns.Contains(columnDbName)) return Result.Ok(spaceData);

		spaceData.Columns.Remove(columnDbName);
		var updateResult = _spaceManager.UpdateSpaceData(spaceData.ToModel());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(updateResult.Errors));

		return Result.Ok(new TucSpaceData(updateResult.Value));

	}

	internal Result<TucSpaceData> UpdateSpaceDataFilters(Guid spaceDataId, List<TucFilterBase> filters)
	{
		if (spaceDataId == Guid.Empty) return Result.Fail("spaceDataId is required");
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) return Result.Fail("spaceData not found");
		spaceData.Filters = filters;
		var model = spaceData.ToModel();
		var updateResult = _spaceManager.UpdateSpaceData(spaceData.ToModel());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(updateResult.Errors));

		return Result.Ok(new TucSpaceData(updateResult.Value));

	}

	//Space view
	internal TucSpaceView GetSpaceView(Guid viewId)
	{
		var serviceResult = _spaceManager.GetSpaceView(viewId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceView failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return null;

		return new TucSpaceView(serviceResult.Value);
	}

	internal List<TucSpaceView> GetSpaceViewList(Guid spaceId)
	{
		var serviceResult = _spaceManager.GetSpaceViewsList(spaceId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceViewsList failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucSpaceView(x)).ToList();
	}


	internal Result<TucSpaceView> CreateSpaceViewWithForm(TucSpaceView view)
	{
		//TODO RUMEN: big part of this needs to be created as a service and be in transaction

		TfSpace space = null;
		TfSpaceData spaceData = null;
		TfSpaceView spaceView = null;
		TfDataProvider dataprovider = null;
		#region << Validate>>
		var validationErrors = new List<ValidationError>();
		//args
		if (String.IsNullOrWhiteSpace(view.Name)) validationErrors.Add(new ValidationError(nameof(view.Name), "required"));
		if (view.SpaceId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(view.SpaceId), "required"));
		if (view.DataSetType == TucSpaceViewDataSetType.New)
		{
			if (String.IsNullOrWhiteSpace(view.NewSpaceDataName)) validationErrors.Add(new ValidationError(nameof(view.NewSpaceDataName), "required"));
			if (view.DataProviderId is null) validationErrors.Add(new ValidationError(nameof(view.DataProviderId), "required"));
		}
		else if (view.DataSetType == TucSpaceViewDataSetType.Existing)
			if (view.SpaceDataId is null) validationErrors.Add(new ValidationError(nameof(view.SpaceDataId), "required"));

		//Space
		var spaceResult = _spaceManager.GetSpace(view.SpaceId);
		if (spaceResult.IsFailed) return Result.Fail(new Error("GetSpace failed").CausedBy(spaceResult.Errors));
		if (spaceResult.Value is null) validationErrors.Add(new ValidationError(nameof(view.SpaceId), "space is not found"));
		space = spaceResult.Value;

		//DataProvider
		if (view.DataProviderId is not null)
		{
			var providerResult = _dataProviderManager.GetProvider(view.DataProviderId.Value);
			if (providerResult.IsFailed) return Result.Fail(new Error("GetProvider failed").CausedBy(providerResult.Errors));
			if (providerResult.Value is null) validationErrors.Add(new ValidationError(nameof(view.DataProviderId), "data provider is not found"));
			dataprovider = providerResult.Value;
		}

		//SpaceData
		if (view.SpaceDataId is not null)
		{
			var spaceDataResult = _spaceManager.GetSpaceData(view.SpaceDataId.Value);
			if (spaceDataResult.IsFailed) return Result.Fail(new Error("GetSpaceData failed").CausedBy(spaceDataResult.Errors));
			if (spaceDataResult.Value is null) validationErrors.Add(new ValidationError(nameof(view.SpaceDataId), "dataset is not found"));
			spaceData = spaceDataResult.Value;
		}

		if (validationErrors.Count > 0)
			return Result.Fail(validationErrors);

		#endregion

		//Should start transaction
		#region << create space data if needed >>
		if (spaceData is null)
		{
			List<string> selectedColumns = new();
			//system columns are always selected so we should not add them in the space data
			if (view.AddProviderColumns && view.AddSharedColumns)
			{
				//all columns are requested from the provider, so send empty column list, which will apply newly added columns
				//to the provider dynamically
			}
			else if (view.AddProviderColumns) selectedColumns.AddRange(dataprovider.Columns.Select(x => x.DbName).ToList());
			else if (view.AddSharedColumns) selectedColumns.AddRange(dataprovider.SharedColumns.Select(x => x.DbName).ToList());

			var spaceDataObj = new TfSpaceData()
			{
				Id = Guid.NewGuid(),
				Name = view.NewSpaceDataName,
				Filters = new(),//filters will not be added at this point
				Columns = selectedColumns,
				DataProviderId = dataprovider.Id,
				SpaceId = space.Id,
				Position = 1 //position is overrided in the creation
			};

			var tfResult = _spaceManager.CreateSpaceData(spaceDataObj);
			if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceData failed").CausedBy(tfResult.Errors));
			if (tfResult.Value is null) return Result.Fail("CreateSpaceData failed to return value");
			spaceData = tfResult.Value;
		}
		#endregion

		#region << create view>>
		{
			var spaceViewObj = new TfSpaceView()
			{
				Id = Guid.NewGuid(),
				Name = view.Name,
				Position = 1,//will be overrided later
				SpaceDataId = spaceData.Id,
				SpaceId = space.Id,
				Type = view.Type.ConvertSafeToEnum<TucSpaceViewType, TfSpaceViewType>()
			};
			var tfResult = _spaceManager.CreateSpaceView(spaceViewObj);
			if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceView failed").CausedBy(tfResult.Errors));
			if (tfResult.Value is null) return Result.Fail("CreateSpaceView failed to return value");
			spaceView = tfResult.Value;
		}
		#endregion

		#region << create view columns>>
		{
			//TBD
		}
		#endregion
		//Should commit transaction

		return Result.Ok(new TucSpaceView(spaceView));
	}

	internal Result<TucSpaceView> UpdateSpaceViewWithForm(TucSpaceView view)
	{
		//TODO RUMEN: big part of this needs to be created as a service and be in transaction
		TfSpace space = null;
		TfSpaceData spaceData = null;
		TfSpaceView spaceView = null;
		TfDataProvider dataprovider = null;
		#region << Validate>>
		var validationErrors = new List<ValidationError>();
		//args
		if (String.IsNullOrWhiteSpace(view.Name)) validationErrors.Add(new ValidationError(nameof(view.Name), "required"));
		if (view.SpaceId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(view.SpaceId), "required"));
		if (view.DataSetType == TucSpaceViewDataSetType.New)
		{
			if (String.IsNullOrWhiteSpace(view.NewSpaceDataName)) validationErrors.Add(new ValidationError(nameof(view.NewSpaceDataName), "required"));
			if (view.DataProviderId is null) validationErrors.Add(new ValidationError(nameof(view.DataProviderId), "required"));
		}
		else if (view.DataSetType == TucSpaceViewDataSetType.Existing)
			if (view.SpaceDataId is null) validationErrors.Add(new ValidationError(nameof(view.SpaceDataId), "required"));

		//Space
		var spaceResult = _spaceManager.GetSpace(view.SpaceId);
		if (spaceResult.IsFailed) return Result.Fail(new Error("GetSpace failed").CausedBy(spaceResult.Errors));
		if (spaceResult.Value is null) validationErrors.Add(new ValidationError(nameof(view.SpaceId), "space is not found"));
		space = spaceResult.Value;

		//DataProvider
		if (view.DataProviderId is not null)
		{
			var providerResult = _dataProviderManager.GetProvider(view.DataProviderId.Value);
			if (providerResult.IsFailed) return Result.Fail(new Error("GetProvider failed").CausedBy(providerResult.Errors));
			if (providerResult.Value is null) validationErrors.Add(new ValidationError(nameof(view.DataProviderId), "data provider is not found"));
			dataprovider = providerResult.Value;
		}

		//SpaceData
		if (view.SpaceDataId is not null)
		{
			var spaceDataResult = _spaceManager.GetSpaceData(view.SpaceDataId.Value);
			if (spaceDataResult.IsFailed) return Result.Fail(new Error("GetSpaceData failed").CausedBy(spaceDataResult.Errors));
			if (spaceDataResult.Value is null) validationErrors.Add(new ValidationError(nameof(view.SpaceDataId), "dataset is not found"));
			spaceData = spaceDataResult.Value;
		}

		if (validationErrors.Count > 0)
			return Result.Fail(validationErrors);

		#endregion

		//Should start transaction
		#region << create space data if needed >>
		if (spaceData is null)
		{
			List<string> selectedColumns = new();
			//system columns are always selected so we should not add them in the space data
			if (view.AddProviderColumns && view.AddSharedColumns)
			{
				//all columns are requested from the provider, so send empty column list, which will apply newly added columns
				//to the provider dynamically
			}
			else if (view.AddProviderColumns) selectedColumns.AddRange(dataprovider.Columns.Select(x => x.DbName).ToList());
			else if (view.AddSharedColumns) selectedColumns.AddRange(dataprovider.SharedColumns.Select(x => x.DbName).ToList());

			var spaceDataObj = new TfSpaceData()
			{
				Id = Guid.NewGuid(),
				Name = view.NewSpaceDataName,
				Filters = new(),//filters will not be added at this point
				Columns = selectedColumns,
				DataProviderId = dataprovider.Id,
				SpaceId = space.Id,
				Position = 1 //position is overrided in the creation
			};

			var tfResult = _spaceManager.CreateSpaceData(spaceDataObj);
			if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceData failed").CausedBy(tfResult.Errors));
			if (tfResult.Value is null) return Result.Fail("CreateSpaceData failed to return value");
			spaceData = tfResult.Value;
		}
		#endregion

		#region << update view>>
		{
			var spaceViewObj = new TfSpaceView()
			{
				Id = view.Id,
				Name = view.Name,
				Position = 1,//will be overrided later
				SpaceDataId = spaceData.Id,
				SpaceId = space.Id,
				Type = view.Type.ConvertSafeToEnum<TucSpaceViewType, TfSpaceViewType>()
			};
			var tfResult = _spaceManager.UpdateSpaceView(spaceViewObj);
			if (tfResult.IsFailed) return Result.Fail(new Error("UpdateSpaceView failed").CausedBy(tfResult.Errors));
			if (tfResult.Value is null) return Result.Fail("UpdateSpaceView failed to return value");
			spaceView = tfResult.Value;
		}
		#endregion

		//Should commit transaction

		return Result.Ok(new TucSpaceView(spaceView));
	}


	//View columns
	internal TucSpaceViewColumn GetViewColumn(Guid columnId)
	{
		var serviceResult = _spaceManager.GetSpaceViewColumn(columnId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceViewColumn failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return new();
		}
		if (serviceResult.Value is null) return new();

		return new TucSpaceViewColumn(serviceResult.Value);

	}

	internal List<TucSpaceViewColumn> GetViewColumns(Guid viewId)
	{
		var serviceResult = _spaceManager.GetSpaceViewColumnsList(viewId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceViewColumnsList failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return new();
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucSpaceViewColumn(x)).ToList();

	}

	internal Result<TucSpaceViewColumn> CreateSpaceViewColumnWithForm(TucSpaceViewColumn column)
	{
		var availableTypes = _spaceManager.GetAvailableSpaceViewColumnTypes().Value;
		var selectedType = availableTypes.FirstOrDefault(x => x.Id == column.ColumnType.Id);
		if (selectedType is null) return Result.Fail("Column selected type not found");
		var result = _spaceManager.CreateSpaceViewColumn(column.ToModel(selectedType));

		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceViewColumn failed").CausedBy(result.Errors));
		return Result.Ok(new TucSpaceViewColumn(result.Value));
	}

	internal Result<TucSpaceViewColumn> UpdateSpaceViewColumnWithForm(TucSpaceViewColumn column)
	{
		var availableTypes = _spaceManager.GetAvailableSpaceViewColumnTypes().Value;
		var selectedType = availableTypes.FirstOrDefault(x => x.Id == column.ColumnType.Id);
		if (selectedType is null) return Result.Fail("Column selected type not found");
		var result = _spaceManager.UpdateSpaceViewColumn(column.ToModel(selectedType));

		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceViewColumn failed").CausedBy(result.Errors));
		return Result.Ok(new TucSpaceViewColumn(result.Value));
	}

	internal Result RemoveSpaceViewColumn(Guid columnId)
	{
		if (columnId == Guid.Empty) return Result.Fail("columnId is required");
		var updateResult = _spaceManager.DeleteSpaceViewColumn(columnId);
		if (updateResult.IsFailed) return Result.Fail(new Error("DeleteSpaceViewColumn failed").CausedBy(updateResult.Errors));
		return Result.Ok();

	}

	internal Result<List<TucSpaceViewColumn>> MoveSpaceViewColumn(Guid viewId, Guid columnId, bool isUp)
	{
		if (columnId == Guid.Empty) return Result.Fail("columnId is required");
		Result updateResult = null;
		if (isUp)
			updateResult = _spaceManager.MoveSpaceViewColumnUp(columnId);
		else
			updateResult = _spaceManager.MoveSpaceViewColumnDown(columnId);

		if (updateResult.IsFailed) return Result.Fail(new Error("MoveSpaceViewColumn failed").CausedBy(updateResult.Errors));
		return Result.Ok(GetViewColumns(viewId));

	}


	//Data provider
	internal List<TucDataProvider> GetDataProviderList()
	{
		var serviceResult = _dataProviderManager.GetProviders();
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviders failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucDataProvider(x)).ToList();
	}

}
