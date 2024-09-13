namespace WebVella.Tefter.Web.Store.SpaceState;

public record ToggleSpaceViewItemSelectionAction
{
	public List<Guid> IdList { get; }
	internal ToggleSpaceViewItemSelectionAction(
		List<Guid> idList
		)
	{
		IdList = idList;
	}
}
