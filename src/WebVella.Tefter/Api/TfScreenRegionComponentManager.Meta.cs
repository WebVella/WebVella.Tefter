namespace WebVella.Tefter;



internal class TfScreenRegionComponentMeta
{
	public ScreenRegion ScreenRegion { get; init; }
	public Type ComponentType { get; init; }
	public int Position { get; init; }

	public string Name { get; init; } = null;
	public string UrlSlug { get; init; } = null;
}