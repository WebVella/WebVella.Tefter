namespace WebVella.Tefter.Addons;

public interface ITfScreenRegionComponent<T> : ITfAddon where T : TfBaseScreenRegionContext
{
	public int PositionRank { get; init;}
	public List<TfScreenRegionScope> Scopes { get; init; }
	T? RegionContext { get; set; }
}

public class TfScreenRegionComponentMeta
{
	public Guid Id { get; init;}
	public int PositionRank { get; init;}
	public string Name { get; init;}
	public string Description { get; init;}
	public string FluentIconName { get; init;}
	public Type Type { get; init; }
	public List<TfScreenRegionScope> Scopes { get; init; }
}