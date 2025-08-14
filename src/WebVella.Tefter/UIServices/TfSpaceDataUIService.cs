namespace WebVella.Tefter.UIServices;

public partial interface ITfSpaceDataUIService
{
	//Events
	event EventHandler<TfSpaceData> SpaceDataCreated;
	event EventHandler<TfSpaceData> SpaceDataUpdated;
	event EventHandler<TfSpaceData> SpaceDataDeleted;

	//Space Data
	List<TfSpaceData> GetAllSpaceData(string? search = null);
	List<TfSpaceData> GetAllSpaceData(Guid spaceId);
	TfSpaceData GetSpaceData(Guid spaceDataId);
	List<TfSpaceData> GetSpaceDataList(Guid spaceId, string? search = null);
	TfSpaceData CreateSpaceData(TfSpaceData item);
	TfSpaceData UpdateSpaceData(TfSpaceData item);
	void DeleteSpaceData(Guid itemId);
	void CopySpaceData(Guid itemId);

	//Columns
	List<TfSpaceDataColumn> GetSpaceDataColumnOptions(Guid spaceDataId);
	List<TfSpaceDataColumn> GetSpaceDataColumns(Guid spaceDataId);
	void AddSpaceDataColumn(Guid spaceDataId, TfSpaceDataColumn column);
	void RemoveSpaceDataColumn(Guid spaceDataId, TfSpaceDataColumn column);

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

	TfDataTable SaveDataDataTable(TfDataTable dt);

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
	public event EventHandler<TfSpaceData> SpaceDataCreated = default!;
	public event EventHandler<TfSpaceData> SpaceDataUpdated = default!;
	public event EventHandler<TfSpaceData> SpaceDataDeleted = default!;
	#endregion

	#region << Space Data>>
	public List<TfSpaceData> GetAllSpaceData(string? search = null)
		=> _tfService.GetAllSpaceData(search);
	public List<TfSpaceData> GetAllSpaceData(Guid spaceId)
		=> _tfService.GetSpaceDataList(spaceId);
	public TfSpaceData GetSpaceData(Guid itemId) => _tfService.GetSpaceData(itemId);
	public List<TfSpaceData> GetSpaceDataList(Guid spaceId, string? search = null) => _tfService.GetSpaceDataList(spaceId, search);
	public TfSpaceData CreateSpaceData(TfSpaceData submit)
	{
		var form = new TfCreateSpaceData
		{
			Columns = submit.Columns,
			DataProviderId = submit.DataProviderId,
			Filters = submit.Filters,
			Id = submit.Id,
			Name = submit.Name,
			SortOrders = submit.SortOrders,
			SpaceId = submit.SpaceId,
		};
		var spaceData = _tfService.CreateSpaceData(form);
		SpaceDataCreated?.Invoke(this, spaceData);
		return spaceData;
	}
	public TfSpaceData UpdateSpaceData(TfSpaceData submit)
	{
		var form = new TfUpdateSpaceData
		{
			Columns = submit.Columns,
			DataProviderId = submit.DataProviderId,
			Filters = submit.Filters,
			Id = submit.Id,
			Name = submit.Name,
			SortOrders = submit.SortOrders
		};
		var spaceData = _tfService.UpdateSpaceData(form);
		SpaceDataUpdated?.Invoke(this, spaceData);
		return spaceData;
	}
	public void DeleteSpaceData(Guid itemId)
	{
		_tfService.DeleteSpaceData(itemId);
		var spaceData = _tfService.GetSpaceData(itemId);
		SpaceDataDeleted?.Invoke(this, spaceData);
	}

	public void CopySpaceData(Guid itemId)
	{
		var spaceData = _tfService.CopySpaceData(itemId);
		SpaceDataCreated?.Invoke(this, spaceData);
	}

	#endregion

	#region << Columns >>

	public List<TfSpaceDataColumn> GetSpaceDataColumnOptions(Guid spaceDataId)
		=> _tfService.GetSpaceDataColumnOptions(spaceDataId);

	public List<TfSpaceDataColumn> GetSpaceDataColumns(Guid spaceDataId)
		=> _tfService.GetSpaceDataColumns(spaceDataId);


	public void AddSpaceDataColumn(Guid spaceDataId, TfSpaceDataColumn column)
	{
		_tfService.AddSpaceDataColumn(spaceDataId, column);
		var spaceData = _tfService.GetSpaceData(spaceDataId);
		SpaceDataUpdated?.Invoke(this, spaceData);
	}
	public void RemoveSpaceDataColumn(Guid spaceDataId, TfSpaceDataColumn column)
	{
		_tfService.RemoveSpaceDataColumn(spaceDataId, column);
		var spaceData = _tfService.GetSpaceData(spaceDataId);
		SpaceDataUpdated?.Invoke(this, spaceData);
	}
	#endregion

	#region << Filters >>
	public void UpdateSpaceDataFilters(Guid spaceDataId,
		List<TfFilterBase> filters)
	{
		_tfService.UpdateSpaceDataFilters(spaceDataId, filters);
		var spaceData = _tfService.GetSpaceData(spaceDataId);
		SpaceDataUpdated?.Invoke(this, spaceData);
	}
	#endregion

	#region << Sorts >>
	public void UpdateSpaceDataSorts(Guid spaceDataId,
		List<TfSort> sorts)
	{
		_tfService.UpdateSpaceDataSorts(spaceDataId, sorts);
		var spaceData = _tfService.GetSpaceData(spaceDataId);
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
	public TfDataTable SaveDataDataTable(TfDataTable dt)
	{
		return _tfService.SaveDataTable(dt);
	}
	#endregion
}
