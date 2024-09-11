namespace WebVella.Tefter.Web.Store.SpaceState;

public partial record SpaceState
{
	internal TucSpaceView SpaceView { get; init; }
	internal TfDataTable SpaceViewData { get; init; }

	internal List<TucSpaceView> SpaceViewList { get; init; } = new();
	internal List<TucSpaceViewColumn> SpaceViewColumnList { get; init; } = new();

	public List<Guid> SelectedDataRows { get; init; } = new();
}
