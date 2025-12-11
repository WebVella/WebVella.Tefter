namespace WebVella.Tefter.Models;

public record TfCreatePinDataBookmarkModel
{
	public List<Guid> SelectedRowIds { get; set; } = new();
	public TfDataset Dataset { get; set; } = null!;
	public TfSpacePage SpacePage { get; set; } = null!;
	public TfSpaceView SpaceView { get; set; } = null!;
	public TfUser User { get; set; } = null!;
	public string Name { get; set; } = null!;
	public string? Description { get; set; } = null;
}    