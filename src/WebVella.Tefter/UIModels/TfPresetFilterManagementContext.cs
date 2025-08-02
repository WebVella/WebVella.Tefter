namespace WebVella.Tefter.Models;

public record TfPresetFilterManagementContext
{
	public TfDataProvider? DataProvider { get; init; }
	public TfSpaceViewPreset? Item { get; init; }
	public List<TfSpaceViewPreset> Parents { get; init; } = new();

	

}
