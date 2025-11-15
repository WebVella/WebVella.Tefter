namespace WebVella.Tefter.Models;

public record TfPresetFilterManagementContext
{
	public TfDataset? DateSet { get; init; }
	public TfSpaceView? SpaceView { get; init; }
	public TfSpaceViewPreset? Item { get; init; }
	public List<TfSpaceViewPreset> Parents { get; init; } = new();

	

}
