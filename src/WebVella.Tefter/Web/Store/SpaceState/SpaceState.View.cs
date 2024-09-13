namespace WebVella.Tefter.Web.Store.SpaceState;

public partial record SpaceState
{
	internal TucSpaceView SpaceView { get; init; }
	internal TfDataTable SpaceViewData { get; init; }

	internal List<TucSpaceView> SpaceViewList { get; init; } = new();
	internal List<TucSpaceViewColumn> SpaceViewColumnList { get; init; } = new();

	public List<Guid> SelectedDataRows { get; init; } = new();
	public bool AllDataRowsSelected
	{
		get
		{
			if(SpaceViewData is null) return false;
			var allSelected = true;
			for (int i = 0; i < SpaceViewData.Rows.Count; i++)
			{
				var rowId = (Guid)SpaceViewData.Rows[i][TfConstants.TEFTER_ITEM_ID_PROP_NAME];
				if(!SelectedDataRows.Contains(rowId))
				{ 
					allSelected = false;
					break;
				}
			}

			return allSelected;
		}
	}
}
