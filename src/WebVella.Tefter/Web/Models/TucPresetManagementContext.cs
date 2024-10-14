namespace WebVella.Tefter.Web.Models;

public record TucPresetManagementContext
{
	public TucSpaceViewPreset Item { get; init; }
	public List<TucSpaceViewPreset> Parents { get; init; }

	

}
