namespace WebVella.Tefter.Models;

public record TfUseTemplateContext
{
	public List<Guid> SelectedRowIds { get; init; } = new();
	public TfDataset SpaceData { get; init; } = default!;
	public TfUser User { get; init; } = default!;
	public ITfTemplatePreviewResult? Preview { get; init; } = null;
}
