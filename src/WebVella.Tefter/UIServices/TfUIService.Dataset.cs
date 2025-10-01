namespace WebVella.Tefter.UIServices;

public partial interface ITfUIService
{
	//Events
	event EventHandler<TfDataset> DatasetCreated;
	event EventHandler<TfDataset> DatasetUpdated;
	event EventHandler<TfDataset> DatasetDeleted;

	//Space Data
	List<TfDataset> GetDatasets(string? search = null, Guid? providerId = null);
	TfDataset? GetDataset(Guid datasetId);
	TfDataset CreateDataset(TfDataset item);
	TfDataset UpdateDataset(TfDataset item);
	void DeleteDataset(Guid itemId);
	void CopyDataset(Guid itemId);

	//Columns
	List<TfDatasetColumn> GetDatasetColumnOptions(Guid datasetId);
	List<TfDatasetColumn> GetDatasetColumns(Guid datasetId);
	void UpdateDatasetColumns(Guid datasetId, List<TfDatasetColumn> columns);

	//Filters
	void UpdateDatasetFilters(Guid datasetId,
		List<TfFilterBase> filters);

	//Sorts
	void UpdateDatasetSorts(Guid datasetId,
		List<TfSort> sorts);

	//Data
	public TfDataTable QueryDataset(
		Guid datasetId,
		List<TfFilterBase>? userFilters = null,
		List<TfSort>? userSorts = null,
		List<TfFilterBase>? presetFilters = null,
		List<TfSort>? presetSorts = null,
		string? search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false,
		bool returnOnlyTfIds = false);

	public List<Guid> QueryDatasetIdList(
		Guid datasetId,
		List<TfFilterBase>? userFilters = null,
		List<TfSort>? userSorts = null,
		List<TfFilterBase>? presetFilters = null,
		List<TfSort>? presetSorts = null,
		string? search = null
		);
	TfDataTable SaveDataTable(TfDataTable dt);
	TfDataTable InsertRowInDataTable(TfDataTable dt);

}
public partial class TfUIService : ITfUIService
{

	#region << Events >>
	public event EventHandler<TfDataset> DatasetCreated = null!;
	public event EventHandler<TfDataset> DatasetUpdated = null!;
	public event EventHandler<TfDataset> DatasetDeleted = null!;
	#endregion

	#region << Data set>>
	public List<TfDataset> GetDatasets(string? search = null, Guid? providerId = null)
		=> _tfService.GetDatasets(search: search, providerId: providerId);
	public TfDataset? GetDataset(Guid datasetId) => _tfService.GetDataset(datasetId);
	public TfDataset CreateDataset(TfDataset submit)
	{
		var form = new TfCreateDataset
		{
			Columns = submit.Columns,
			DataProviderId = submit.DataProviderId,
			Filters = submit.Filters,
			Id = submit.Id,
			Name = submit.Name,
			SortOrders = submit.SortOrders
		};
		var spaceData = _tfService.CreateDataset(form);
		DatasetCreated?.Invoke(this, spaceData);
		return spaceData;
	}
	public TfDataset UpdateDataset(TfDataset submit)
	{
		var form = new TfUpdateDataset
		{
			Columns = submit.Columns,
			DataProviderId = submit.DataProviderId,
			Filters = submit.Filters,
			Id = submit.Id,
			Name = submit.Name,
			SortOrders = submit.SortOrders
		};
		var spaceData = _tfService.UpdateDataset(form);
		DatasetUpdated?.Invoke(this, spaceData);
		return spaceData;
	}
	public void DeleteDataset(Guid itemId)
	{
		_tfService.DeleteDataset(itemId);
		var dataset = _tfService.GetDataset(itemId);
		DatasetDeleted?.Invoke(this, dataset);
	}

	public void CopyDataset(Guid itemId)
	{
		var dataset = _tfService.CopyDataset(itemId);
		DatasetCreated?.Invoke(this, dataset);
	}
	#endregion

	#region << Columns >>

	public List<TfDatasetColumn> GetDatasetColumnOptions(Guid datasetId)
		=> _tfService.GetDatasetColumnOptions(datasetId);

	public List<TfDatasetColumn> GetDatasetColumns(Guid datasetId)
		=> _tfService.GetDatasetColumns(datasetId);


	public void UpdateDatasetColumns(Guid datasetId, List<TfDatasetColumn> columns)
	{
		_tfService.UpdataDatasetColumns(datasetId, columns);
		var dataset = _tfService.GetDataset(datasetId);
		DatasetUpdated?.Invoke(this, dataset!);
	}

	#endregion

	#region << Filters >>
	public void UpdateDatasetFilters(Guid datasetId,
		List<TfFilterBase> filters)
	{
		_tfService.UpdateDatasetFilters(datasetId, filters);
		var dataset = _tfService.GetDataset(datasetId);
		DatasetUpdated?.Invoke(this, dataset!);
	}
	#endregion

	#region << Sorts >>
	public void UpdateDatasetSorts(Guid datasetId,
		List<TfSort> sorts)
	{
		_tfService.UpdateDatasetSorts(datasetId, sorts);
		var dataset = _tfService.GetDataset(datasetId);
		DatasetUpdated?.Invoke(this, dataset!);
	}
	#endregion

	#region << Data >>
	public TfDataTable QueryDataset(
			Guid datasetId,
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
			datasetId: datasetId,
			userFilters: userFilters,
			userSorts: userSorts,
			presetFilters: presetFilters,
			presetSorts: presetSorts,
			search: search,
			page: page,
			pageSize: pageSize,
			noRows: noRows,
			returnOnlyTfIds: returnOnlyTfIds);


	public List<Guid> QueryDatasetIdList(
			Guid datasetId,
			List<TfFilterBase>? userFilters = null,
			List<TfSort>? userSorts = null,
			List<TfFilterBase>? presetFilters = null,
			List<TfSort>? presetSorts = null,
			string? search = null)
	{
		var result = new List<Guid>();
		var dt = _tfService.QuerySpaceData(
					datasetId: datasetId,
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
