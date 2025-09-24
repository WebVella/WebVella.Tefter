namespace WebVella.Tefter.UIServices;

public partial interface ITfSpaceDataUIService
{
	//Events
	event EventHandler<TfDataSet> SpaceDataCreated;
	event EventHandler<TfDataSet> SpaceDataUpdated;
	event EventHandler<TfDataSet> SpaceDataDeleted;

	//Space Data
	List<TfDataSet> GetAllSpaceData(string? search = null);
	List<TfDataSet> GetAllSpaceData(Guid spaceId);
	TfDataSet GetSpaceData(Guid spaceDataId);
	List<TfDataSet> GetSpaceDataList(Guid spaceId, string? search = null);
	TfDataSet CreateSpaceData(TfDataSet item);
	TfDataSet UpdateSpaceData(TfDataSet item);
	void DeleteSpaceData(Guid itemId);
	void CopySpaceData(Guid itemId);

	//Columns
	List<TfDataSetColumn> GetSpaceDataColumnOptions(Guid spaceDataId);
	List<TfDataSetColumn> GetSpaceDataColumns(Guid spaceDataId);
	void AddSpaceDataColumn(Guid spaceDataId, TfDataSetColumn column);
	void AddAvailableColumnsToSpaceData(Guid spaceDataId);
	void RemoveSpaceDataColumn(Guid spaceDataId, TfDataSetColumn column);

	//Filters
	void UpdateSpaceDataFilters(Guid spaceDataId,
		List<TfFilterBase> filters);

	//Sorts
	void UpdateSpaceDataSorts(Guid spaceDataId,
		List<TfSort> sorts);

	//Data
	public TfDataTable QuerySpaceData(
		Guid spaceDataId,
		List<TfFilterBase>? userFilters = null,
		List<TfSort>? userSorts = null,
		List<TfFilterBase>? presetFilters = null,
		List<TfSort>? presetSorts = null,
		string? search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false,
		bool returnOnlyTfIds = false);

	public List<Guid> QuerySpaceDataIdList(
		Guid spaceDataId,
		List<TfFilterBase>? userFilters = null,
		List<TfSort>? userSorts = null,
		List<TfFilterBase>? presetFilters = null,
		List<TfSort>? presetSorts = null,
		string? search = null
		);

	TfDataTable SaveDataTable(TfDataTable dt);
	TfDataTable InsertRowInDataTable(TfDataTable dt);

}
public partial class TfSpaceDataUIService : ITfSpaceDataUIService
{
	#region << Ctor >>
	private static readonly AsyncLock _asyncLock = new AsyncLock();
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfSpaceDataUIService> LOC;

	public TfSpaceDataUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfSpaceDataUIService>>() ?? default!;
	}
	#endregion

	#region << Events >>
	public event EventHandler<TfDataSet> SpaceDataCreated = default!;
	public event EventHandler<TfDataSet> SpaceDataUpdated = default!;
	public event EventHandler<TfDataSet> SpaceDataDeleted = default!;
	#endregion

	#region << Space Data>>
	public List<TfDataSet> GetAllSpaceData(string? search = null)
		=> _tfService.GetAllDataSets(search);
	public List<TfDataSet> GetAllSpaceData(Guid spaceId)
		=> _tfService.GetDataSetList(spaceId);
	public TfDataSet GetSpaceData(Guid itemId) => _tfService.GetDataSet(itemId);
	public List<TfDataSet> GetSpaceDataList(Guid spaceId, string? search = null) => _tfService.GetDataSetList(spaceId, search);
	public TfDataSet CreateSpaceData(TfDataSet submit)
	{
		var form = new TfCreateDataSet
		{
			Columns = submit.Columns,
			DataProviderId = submit.DataProviderId,
			Filters = submit.Filters,
			Id = submit.Id,
			Name = submit.Name,
			SortOrders = submit.SortOrders
		};
		var spaceData = _tfService.CreateDataSet(form);
		SpaceDataCreated?.Invoke(this, spaceData);
		return spaceData;
	}
	public TfDataSet UpdateSpaceData(TfDataSet submit)
	{
		var form = new TfUpdateDataSet
		{
			Columns = submit.Columns,
			DataProviderId = submit.DataProviderId,
			Filters = submit.Filters,
			Id = submit.Id,
			Name = submit.Name,
			SortOrders = submit.SortOrders
		};
		var spaceData = _tfService.UpdateDataSet(form);
		SpaceDataUpdated?.Invoke(this, spaceData);
		return spaceData;
	}
	public void DeleteSpaceData(Guid itemId)
	{
		_tfService.DeleteDataSet(itemId);
		var spaceData = _tfService.GetDataSet(itemId);
		SpaceDataDeleted?.Invoke(this, spaceData);
	}

	public void CopySpaceData(Guid itemId)
	{
		var spaceData = _tfService.CopyDataSet(itemId);
		SpaceDataCreated?.Invoke(this, spaceData);
	}

	#endregion

	#region << Columns >>

	public List<TfDataSetColumn> GetSpaceDataColumnOptions(Guid spaceDataId)
		=> _tfService.GetDataSetColumnOptions(spaceDataId);

	public List<TfDataSetColumn> GetSpaceDataColumns(Guid spaceDataId)
		=> _tfService.GetDataSetColumns(spaceDataId);


	public void AddSpaceDataColumn(Guid spaceDataId, TfDataSetColumn column)
	{
		_tfService.AddDataSetColumn(spaceDataId, column);
		var spaceData = _tfService.GetDataSet(spaceDataId);
		SpaceDataUpdated?.Invoke(this, spaceData);
	}

	public void AddAvailableColumnsToSpaceData(Guid spaceDataId)
	{
		_tfService.AddAvailableColumnsToDataSet(spaceDataId);
		var spaceData = _tfService.GetDataSet(spaceDataId);
		SpaceDataUpdated?.Invoke(this, spaceData);
	}
	public void RemoveSpaceDataColumn(Guid spaceDataId, TfDataSetColumn column)
	{
		_tfService.RemoveDataSetColumn(spaceDataId, column);
		var spaceData = _tfService.GetDataSet(spaceDataId);
		SpaceDataUpdated?.Invoke(this, spaceData);
	}
	#endregion

	#region << Filters >>
	public void UpdateSpaceDataFilters(Guid spaceDataId,
		List<TfFilterBase> filters)
	{
		_tfService.UpdateDataSetFilters(spaceDataId, filters);
		var spaceData = _tfService.GetDataSet(spaceDataId);
		SpaceDataUpdated?.Invoke(this, spaceData);
	}
	#endregion

	#region << Sorts >>
	public void UpdateSpaceDataSorts(Guid spaceDataId,
		List<TfSort> sorts)
	{
		_tfService.UpdateDataSetSorts(spaceDataId, sorts);
		var spaceData = _tfService.GetDataSet(spaceDataId);
		SpaceDataUpdated?.Invoke(this, spaceData);
	}
	#endregion

	#region << Data >>
	public TfDataTable QuerySpaceData(
			Guid spaceDataId,
			List<TfFilterBase>? userFilters = null,
			List<TfSort>? userSorts = null,
			List<TfFilterBase>? presetFilters = null,
			List<TfSort>? presetSorts = null,
			string? search = null,
			int? page = null,
			int? pageSize = null,
			bool noRows = false,
			bool returnOnlyTfIds = false)
		=> _tfService.QuerySpaceData(
			spaceDataId: spaceDataId,
			userFilters: userFilters,
			userSorts: userSorts,
			presetFilters: presetFilters,
			presetSorts: presetSorts,
			search: search,
			page: page,
			pageSize: pageSize,
			noRows: noRows,
			returnOnlyTfIds: returnOnlyTfIds);


	public List<Guid> QuerySpaceDataIdList(
			Guid spaceDataId,
			List<TfFilterBase>? userFilters = null,
			List<TfSort>? userSorts = null,
			List<TfFilterBase>? presetFilters = null,
			List<TfSort>? presetSorts = null,
			string? search = null)
	{
		var result = new List<Guid>();
		var dt = _tfService.QuerySpaceData(
					spaceDataId: spaceDataId,
					userFilters: userFilters,
					userSorts: userSorts,
					presetFilters: presetFilters,
					presetSorts: presetSorts,
					search: search,
					page: null,
					pageSize: null,
					noRows: false,
					returnOnlyTfIds: true);

		for (int i = 0; i < dt.Rows.Count; i++)
		{
			result.Add((Guid)dt.Rows[i][TfConstants.TEFTER_ITEM_ID_PROP_NAME]);
		}
		return result;
	}
	public TfDataTable SaveDataTable(TfDataTable dt)
	{
		return _tfService.SaveDataTable(dt);
	}

	public TfDataTable InsertRowInDataTable(TfDataTable dt)
	{
		var newDt = dt.NewTable();
		var newRow = newDt.NewRow();
		newDt.Rows.Add(newRow);
		return SaveDataTable(newDt);
	}
	#endregion
}
