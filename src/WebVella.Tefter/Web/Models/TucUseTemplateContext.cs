namespace WebVella.Tefter.Web.Models;

public record TucUseTemplateContext
{
	public Guid? TemplateId { get; init; }
	public List<Guid> SelectedRowIds { get; init; }
	public TucSpaceData SpaceData { get; init; }
	public TucUser User { get; init; }
	public ITfTemplatePreviewResult Preview { get; init; }
}
