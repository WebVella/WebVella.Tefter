namespace WebVella.Tefter.Web.Store.SpaceState;

public partial record SpaceState
{
	internal TucSpaceView SpaceView { get; init; }
	internal TfDataTable SpaceViewData { get; init; }

	internal List<TucSpaceView> SpaceViewList { get; init; } = new();
	internal List<TucSpaceViewColumn> SpaceViewColumnList { get; init; } = new();

	public List<Guid> SelectedDataRows { get; init; } = new();
	public bool? SelectionState
	{
		get
		{
			if(SpaceViewData is null) return false;
			var atLeastOneSelected = false;
			var atLeastOnNotSelected = false;
			foreach (var row in SpaceViewData.Rows)
			{
				
			}

			return false;
		}
	}
}
