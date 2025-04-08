namespace WebVella.Tefter.Models;

public interface ITfRegionComponent<T> where T : TfBaseScreenRegion
{
	public Guid Id { get; init;}
	public int PositionRank { get; init;}
	public string Name { get; init;}
	public string Description { get; init;}
	public string FluentIconName { get; init; }
	public List<TfScreenRegionScope> Scopes { get; init; }
	T Context { get; init; }
}

public class TfRegionComponentMeta
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