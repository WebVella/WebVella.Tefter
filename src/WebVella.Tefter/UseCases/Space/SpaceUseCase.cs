using WebVella.Tefter.Web.Components.SearchSpaceDialog;
using WebVella.Tefter.Web.Components.SpaceDataFilterManageDialog;
using WebVella.Tefter.Web.Components.SpaceDataManage;
using WebVella.Tefter.Web.Components.SpaceDetails;
using WebVella.Tefter.Web.Components.SpaceManageDialog;
using WebVella.Tefter.Web.Components.SpaceStateManager;
using WebVella.Tefter.Web.Components.SpaceViewCreateDialog;

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
	internal List<TucDataProvider> AllDataProviders { get; set; } = new();
	internal async Task Init(Type type, Guid? spaceId = null)
	{
		if (type == typeof(TfSpaceStateManager)) await InitForState();
		else if (type == typeof(TfSpaceManageDialog)) await InitSpaceManageDialog();
		else if (type == typeof(TfSpaceDataManage)) await InitSpaceDataManage();
		else if (type == typeof(TfSpaceViewCreateDialog)) await InitSpaceViewManageDialog(spaceId.Value);
		else if (type == typeof(TfSearchSpaceDialog)) await InitForSearchSpace();
		else if (type == typeof(TfSpaceDataFilterManageDialog)) await InitSpaceDataFilterManageDialog();
		else if (type == typeof(TfSpaceDetails)) {}
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
		TfSpace space = null;
		TfSpaceData spaceData = null;
		TfSpaceView spaceView = null;
		TfDataProvider dataprovider = null;
		#region << Validate>>
		var validationErrors = new List<ValidationError>();
		//args
		if (String.IsNullOrWhiteSpace(view.Name)) validationErrors.Add(new ValidationError(nameof(view.Name), "name is required"));
		if (view.SpaceId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(view.SpaceId), "space is required"));
		if (view.DataSetType == TucSpaceViewDataSetType.New)
		{
			if (String.IsNullOrWhiteSpace(view.NewSpaceDataName)) validationErrors.Add(new ValidationError(nameof(view.NewSpaceDataName), "dataset name is required"));
			if (view.DataProviderId is null) validationErrors.Add(new ValidationError(nameof(view.DataProviderId), "dataprovider is required"));
		}
		else if (view.DataSetType == TucSpaceViewDataSetType.Existing)
			if (view.SpaceDataId is null) validationErrors.Add(new ValidationError(nameof(view.SpaceDataId), "existing dataset is required"));

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
			if (view.AddProviderColumns) selectedColumns.AddRange(dataprovider.Columns.Select(x => x.DbName).ToList());
			if (view.AddSharedColumns) selectedColumns.AddRange(dataprovider.SharedColumns.Select(x => x.DbName).ToList());

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
