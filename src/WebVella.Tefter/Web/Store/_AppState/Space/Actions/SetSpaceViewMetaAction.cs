namespace WebVella.Tefter.Web.Store;

public record SetSpaceViewMetaAction : TfBaseAction
{
	public List<TucSpaceViewColumn> SpaceViewColumns { get; }

	internal SetSpaceViewMetaAction(
		TfBaseComponent component,
		List<TucSpaceViewColumn> spaceViewColumns
		)
	{
		Component = component;
		SpaceViewColumns = spaceViewColumns;
	}
}
