namespace WebVella.Tefter.Web.Store.CultureState;

[FeatureState]
public record CultureState
{
	[JsonIgnore]
	internal StateUseCase UseCase { get; init; }
	public TucCultureOption Culture { get; init; }

}
