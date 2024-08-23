namespace WebVella.Tefter.Web.Store.SpaceState;

public partial record SpaceState
{
	internal TucSpaceView SpaceView { get; init; }

	internal List<TucSpaceView> SpaceViewList { get; init; } = new();
}
