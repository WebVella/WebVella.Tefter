namespace WebVella.Tefter.Web.Store;
public partial record TfAppState
{
	public List<TucSpaceView> SpaceViewList { get; init; } = new();
	public TucSpaceView SpaceView { get; init; }
	public TfDataTable SpaceViewData { get; init; }
	public List<TucSpaceViewColumn> SpaceViewColumns { get; init; } = new();
	public List<TucFilterBase> SpaceViewFilters { get; init; }
	public List<TucSort> SpaceViewSorts { get; init; }
	public List<Guid> SelectedDataRows { get; init; } = new();
	public bool AllDataRowsSelected
	{
		get
		{
			if (SpaceViewData is null || SpaceViewData.Rows.Count == 0) return false;
			var allSelected = true;
			for (int i = 0; i < SpaceViewData.Rows.Count; i++)
			{
				var rowId = (Guid)SpaceViewData.Rows[i][TfConstants.TEFTER_ITEM_ID_PROP_NAME];
				if (!SelectedDataRows.Contains(rowId))
				{
					allSelected = false;
					break;
				}
			}

			return allSelected;
		}
	}
	public List<TucSpaceViewColumnType> AvailableColumnTypes { get; init; }
}
