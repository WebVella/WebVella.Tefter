namespace WebVella.Tefter.Web.Store;

public record SetSpaceViewAction : TfBaseAction
{
	public TucSpaceView SpaceView { get; }
	public List<TucSpaceView> SpaceViewList { get; }

	internal SetSpaceViewAction(
	TfBaseComponent component,
		TucSpaceView spaceView,
		List<TucSpaceView> spaceViewList
		)
	{
		Component = component;
		SpaceView = spaceView;
		SpaceViewList = spaceViewList;
	}
}
