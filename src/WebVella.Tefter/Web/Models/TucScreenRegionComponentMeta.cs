namespace WebVella.Tefter.Web.Models;

public record TucScreenRegionComponentMeta
{
	public ScreenRegion Region { get; init; } = ScreenRegion.Pages;
	public int Position { get; init; }
	public string Name { get; init; }
	public string Slug { get; init; }
	public Type ComponentType { get; init; }

}
