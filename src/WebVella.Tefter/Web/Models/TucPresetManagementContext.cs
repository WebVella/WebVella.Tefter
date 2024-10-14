namespace WebVella.Tefter.Web.Models;

public record TucPresetManagementContext
{
	public TucDataProvider DataProvider { get; init; }
	public TucSpaceViewPreset Item { get; init; }
	public List<TucSpaceViewPreset> Parents { get; init; }

	

}
