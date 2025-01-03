namespace WebVella.Tefter.Web.Models;

public record TucUseTemplateContext
{
	public List<Guid> SelectedRowIds { get; init; }
	public TucSpaceData SpaceData { get; init; }
	public TucUser User { get; init; }
	public string SpaceColorString { get; init; }
	public string SpaceBackgroundColorString { get; init; }
	public string SpaceGridSelectedColor { get; init; }
	public string SpaceBorderColor { get; init; }
}
