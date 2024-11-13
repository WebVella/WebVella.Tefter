namespace WebVella.Tefter.Web.Models;

public record TucScreenRegionComponentMeta
{
	public Guid Id { get; init; }
	public TfScreenRegion Region { get; init; } = TfScreenRegion.Pages;
	public int Position { get; init; }
	public string Name { get; init; }
	public string FluentIconName { get; init; }
	public Type ComponentType { get; init; }

}
