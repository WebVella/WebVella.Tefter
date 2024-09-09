namespace WebVella.Tefter.Web.Store.SpaceState;

public record SetSpaceViewMetaAction
{
	public List<TucSpaceViewColumn> SpaceViewColumns { get; }

	internal SetSpaceViewMetaAction(
		List<TucSpaceViewColumn> spaceViewColumns
		)
	{
		SpaceViewColumns = spaceViewColumns;
	}
}
