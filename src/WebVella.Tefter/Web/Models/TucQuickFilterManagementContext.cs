namespace WebVella.Tefter.Web.Models;

public record TucQuickFilterManagementContext
{
	public TucDataProvider DataProvider { get; init; }
	public TucSpaceViewPreset Item { get; init; }
	public List<TucSpaceViewPreset> Parents { get; init; }

	

}
