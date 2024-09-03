namespace WebVella.Tefter.Web.Store.SpaceState;

public record SetSpaceViewAction
{
	public TucSpaceView SpaceView { get; }
	public List<TucSpaceView> SpaceViewList { get; }

	internal SetSpaceViewAction(
		TucSpaceView spaceView,
		List<TucSpaceView> spaceViewList
		)
	{
		SpaceView = spaceView;
		SpaceViewList = spaceViewList;
	}
}
