namespace WebVella.Tefter.Web.Models;

public record TucUseTemplateContext
{
	public List<Guid> SelectedRowIds { get; init; }
	public TucSpaceData SpaceData { get; init; }
	public TucUser User { get; init; }
}
