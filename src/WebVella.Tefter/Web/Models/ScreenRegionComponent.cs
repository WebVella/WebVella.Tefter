namespace WebVella.Tefter.Web.Models;

public record ScreenRegionComponent
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public ScreenRegion Region { get; init; } = ScreenRegion.SpaceViewActions;
	public int Position { get; init; } = 0;
	public Type ComponentType { get; init; }

}
