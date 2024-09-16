namespace WebVella.Tefter.Web.Store;

public record ToggleSpaceViewItemSelectionAction : TfBaseAction
{
	public List<Guid> IdList { get; }
	internal ToggleSpaceViewItemSelectionAction(
		TfBaseComponent component,
		List<Guid> idList
		)
	{
		Component = component;
		IdList = idList;
	}
}
