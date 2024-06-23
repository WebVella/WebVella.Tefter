namespace WebVella.Tefter.Web.Store.CultureState;

[FeatureState]
public record CultureState
{
	public CultureOption Culture { get; init; }
}
