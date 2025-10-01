namespace WebVella.Tefter.Models;

public record TfUseTemplateContext
{
	public List<Guid> SelectedRowIds { get; init; } = new();
	public TfDataset SpaceData { get; init; } = null!;
	public TfUser User { get; init; } = null!;
	public ITfTemplatePreviewResult? Preview { get; init; } = null;
}
