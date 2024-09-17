namespace WebVella.Tefter.Web.Store;
using SystemColor = System.Drawing.Color;

public partial record TfAppState
{
	public List<TucSpaceView> SpaceViewList { get; init; } = new();
	public TucSpaceView SpaceView { get; init; }
	public TfDataTable SpaceViewData { get; init; }
	public List<TucSpaceViewColumn> SpaceViewColumns { get; init; } = new();
	public int Page { get; init; } = 1;
	public int PageSize { get; init; } = TfConstants.PageSize;
	public string SearchQuery { get; init; }
	public List<TucFilterBase> Filters { get; init; }
	public List<TucSort> Sorts { get; init; }
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

}
