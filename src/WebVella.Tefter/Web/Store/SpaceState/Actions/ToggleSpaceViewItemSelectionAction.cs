namespace WebVella.Tefter.Web.Store.SpaceState;

public record ToggleSpaceViewItemSelectionAction
{
	public List<Guid> IdList { get; }
	public bool IsSelected { get; }

	internal ToggleSpaceViewItemSelectionAction(
		List<Guid> idList,
		bool isSelected
		)
	{
		IdList = idList;
		IsSelected = isSelected;
	}
}
