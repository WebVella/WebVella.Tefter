namespace WebVella.Tefter.Web.Store.CultureState;

[FeatureState]
public record CultureState
{
	public TucCultureOption Culture { get; init; }

}
