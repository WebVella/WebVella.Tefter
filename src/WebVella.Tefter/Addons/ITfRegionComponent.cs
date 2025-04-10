namespace WebVella.Tefter.Addons;

public interface ITfRegionComponent<T> : ITfAddon where T : TfBaseScreenRegion
{
	public int PositionRank { get; init;}
	public List<TfScreenRegionScope> Scopes { get; init; }
	T Context { get; init; }
}

public class TfScreenRegionComponentMeta
{
	public Guid Id { get; init;}
	public int PositionRank { get; init;}
	public string Name { get; init;}
	public string Description { get; init;}
	public string FluentIconName { get; init;}
	public Type ComponentType { get; init; }
	public List<TfScreenRegionScope> Scopes { get; init; }
	public List<string> ScopeTypeFullNames { get; init; } = new();
}