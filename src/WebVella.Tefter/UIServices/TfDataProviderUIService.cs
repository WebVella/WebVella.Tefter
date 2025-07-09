namespace WebVella.Tefter.UIServices;

public partial interface ITfDataProviderUIService
{
	//Events
	event EventHandler<TfDataProvider> DataProviderCreated;
	event EventHandler<TfDataProvider> DataProviderUpdated;
	event EventHandler<TfDataProvider> DataProviderDeleted;

	//Data provider
	TfDataProvider GetDataProvider(Guid providerId);
	ReadOnlyCollection<TfDataProvider> GetDataProviders(string? search = null);
	TfDataProvider CreateDataProvider(TfCreateDataProvider providerModel);
	TfDataProvider UpdateDataProvider(TfUpdateDataProvider providerModel);
	void DeleteDataProvider(Guid id);

	List<TfDataProvider> GetDataProviderConnectedProviders(Guid id);

	//Data provider column
	ReadOnlyCollection<DatabaseColumnTypeInfo> GetDatabaseColumnTypeInfosList();
	string? GetDatabaseColumnTypeInfo(TfDatabaseColumnType columnType);
	TfDataProvider CreateDataProviderColumn(TfDataProviderColumn column);
	TfDataProvider UpdateDataProviderColumn(TfDataProviderColumn column);
	TfDataProvider DeleteDataProviderColumn(Guid columnId);
	TfDataProviderSourceSchemaInfo GetDataProviderSourceSchemaInfo(TfDataProvider provider);
	TfDataProvider CreateBulkDataProviderColumn(
		Guid providerId,
		List<TfDataProviderColumn> columns);

	//Data provider identity
	TfDataProvider CreateDataProviderIdentity(TfDataProviderIdentity identity);
	TfDataProvider UpdateDataProviderIdentity(TfDataProviderIdentity identity);
	TfDataProvider DeleteDataProviderIdentity(Guid id);

	//Sync
	DateTime? GetDataProviderNextSynchronizationTime(Guid providerId);
	List<TfDataProviderSynchronizeTask> GetDataProviderSynchronizationTasks(Guid providerId, int? page = null, int? pageSize = null);
	void TriggerSynchronization(Guid providerId);
	TfDataProvider UpdateDataProviderSunchronization(Guid providerId, short syncScheduleMinutes, bool syncScheduleEnabled);
	TfDataProvider UpdateDataProviderSynchPrimaryKeyColumns(Guid providerId, List<string> columns);

	//Data
	long GetDataProviderRowsCount(Guid dataProviderId);
	void DeleteAllProviderRows(Guid dataProviderId);

	TfDataTable QueryDataProvider(Guid providerId, string search = null, int? page = null, int? pageSize = null);
}
public partial class TfDataProviderUIService : ITfDataProviderUIService
{
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfDataProviderUIService> LOC;

	public TfDataProviderUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfDataProviderUIService>>() ?? default!;
	}

	#region << Events >>
	public event EventHandler<TfDataProvider> DataProviderCreated = default!;
	public event EventHandler<TfDataProvider> DataProviderUpdated = default!;
	public event EventHandler<TfDataProvider> DataProviderDeleted = default!;
	#endregion

	#region << Data Provider>>
	public ReadOnlyCollection<TfDataProvider> GetDataProviders(string? search = null)
		=> _tfService.GetDataProviders(search);
	public TfDataProvider GetDataProvider(Guid providerId)
		=> _tfService.GetDataProvider(providerId);
	public TfDataProvider CreateDataProvider(TfCreateDataProvider providerModel)
	{
		var dataProvider = _tfService.CreateDataProvider(providerModel);
		DataProviderCreated?.Invoke(this, dataProvider);
		return dataProvider;
	}
	public TfDataProvider UpdateDataProvider(TfUpdateDataProvider providerModel)
	{
		var dataProvider = _tfService.UpdateDataProvider(providerModel);
		DataProviderUpdated?.Invoke(this, dataProvider);
		return dataProvider;
	}
	public void DeleteDataProvider(Guid id)
	{
		var provider = GetDataProvider(id);
		_tfService.DeleteDataProvider(id);
		DataProviderDeleted?.Invoke(this, provider);
	}

	public List<TfDataProvider> GetDataProviderConnectedProviders(Guid id)
		=> _tfService.GetDataProviderConnectedProviders(id);
	#endregion

	#region << Data Provider Columns >>

	public ReadOnlyCollection<DatabaseColumnTypeInfo> GetDatabaseColumnTypeInfosList()
		=> TfService.GetDatabaseColumnTypeInfosList();
	public string? GetDatabaseColumnTypeInfo(TfDatabaseColumnType columnType)
	{
		var columnTypes = _tfService.GetDatabaseColumnTypeInfos();
		return columnTypes.FirstOrDefault(x => x.Type == columnType)?.Name;
	}

	public TfDataProvider CreateDataProviderColumn(TfDataProviderColumn column)
	{
		var provider = _tfService.CreateDataProviderColumn(column);
		DataProviderUpdated?.Invoke(this, provider);
		return provider;
	}
	public TfDataProvider UpdateDataProviderColumn(TfDataProviderColumn column)
	{
		var provider = _tfService.UpdateDataProviderColumn(column);
		DataProviderUpdated?.Invoke(this, provider);
		return provider;
	}
	public TfDataProvider DeleteDataProviderColumn(Guid columnId)
	{
		var provider = _tfService.DeleteDataProviderColumn(columnId);
		DataProviderUpdated?.Invoke(this, provider);
		return provider;
	}
	public TfDataProviderSourceSchemaInfo GetDataProviderSourceSchemaInfo(TfDataProvider provider)
		=> _tfService.GetDataProviderSourceSchemaInfo(provider.Id);

	public TfDataProvider CreateBulkDataProviderColumn(
		Guid providerId,
		List<TfDataProviderColumn> columns)
	{
		var provider = _tfService.CreateBulkDataProviderColumn(providerId, columns);
		DataProviderUpdated?.Invoke(this, provider);
		return provider;
	}

	#endregion

	#region << Data Provider Identity>>
	public TfDataProvider CreateDataProviderIdentity(TfDataProviderIdentity identity)
	{
		var provider = _tfService.CreateDataProviderIdentity(identity);
		DataProviderCreated?.Invoke(this, provider);
		return provider;
	}

	public TfDataProvider UpdateDataProviderIdentity(TfDataProviderIdentity identity)
	{
		var provider = _tfService.UpdateDataProviderIdentity(identity);
		DataProviderUpdated?.Invoke(this, provider);
		return provider;
	}

	public TfDataProvider DeleteDataProviderIdentity(Guid id)
	{
		var provider = _tfService.DeleteDataProviderIdentity(id);
		DataProviderUpdated?.Invoke(this, provider);
		return provider;
	}
	#endregion

	#region << Sync >>
	public DateTime? GetDataProviderNextSynchronizationTime(Guid providerId)
		=> _tfService.GetDataProviderNextSynchronizationTime(providerId);

	public List<TfDataProviderSynchronizeTask> GetDataProviderSynchronizationTasks(Guid providerId, int? page = null, int? pageSize = null)
		=> _tfService.GetDataProviderSynchronizationTasks(providerId, page, pageSize);

	public void TriggerSynchronization(Guid providerId)
	{
		_tfService.TriggerSynchronization(providerId);
		var dataProvider = _tfService.GetDataProvider(providerId);
		DataProviderUpdated?.Invoke(this, dataProvider);
	}

	public TfDataProvider UpdateDataProviderSunchronization(Guid providerId, short syncScheduleMinutes, bool syncScheduleEnabled)
	{
		_tfService.UpdateDataProviderSunchronization(providerId, syncScheduleMinutes, syncScheduleEnabled);
		var dataProvider = _tfService.GetDataProvider(providerId);
		DataProviderUpdated?.Invoke(this, dataProvider);
		return dataProvider;
	}
	public TfDataProvider UpdateDataProviderSynchPrimaryKeyColumns(Guid providerId, List<string> columns)
	{
		_tfService.UpdateDataProviderSynchPrimaryKeyColumns(providerId, columns);
		var dataProvider = _tfService.GetDataProvider(providerId);
		DataProviderUpdated?.Invoke(this, dataProvider);
		return dataProvider;
	}

	#endregion

	#region << Data >>
	public long GetDataProviderRowsCount(Guid dataProviderId)
		=> _tfService.GetDataProviderRowsCount(dataProviderId);

	public void DeleteAllProviderRows(Guid dataProviderId)
		=> _tfService.DeleteAllProviderRows(dataProviderId);

	public TfDataTable QueryDataProvider(Guid providerId, string search = null, int? page = null, int? pageSize = null)
	=> _tfService.QueryDataProvider(providerId, search, page, pageSize);
	#endregion
}
