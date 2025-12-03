namespace WebVella.Tefter.Addons;

public interface ITfScreenRegionAddon<T> : ITfAddon where T : TfBaseScreenRegion
{
	public int PositionRank { get; init;}
	public List<TfScreenRegionScope> Scopes { get; init; }
	T? RegionContext { get; set; }
}

public class TfScreenRegionComponentMeta
{
	public Guid Id { get; init;}
	public int PositionRank { get; init;}
	public string Name { get; init; } = null!;
	// ReSharper disable once UnusedAutoPropertyAccessor.Global
	public string Description { get; init;} = null!;
	// ReSharper disable once UnusedAutoPropertyAccessor.Global
	public string FluentIconName { get; init;} = null!;
	public Type Type { get; init; } = null!;
	public List<TfScreenRegionScope> Scopes { get; init; } = new();
}