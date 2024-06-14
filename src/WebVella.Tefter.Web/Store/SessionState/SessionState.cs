namespace WebVella.Tefter.Web.Store.SessionState;

[FeatureState]
public record SessionState
{
	public bool IsLoading { get; init; } = true;
	public bool IsDataLoading { get; init; }
	public Guid DataHashId { get; init; }
	public Space Space { get; init; }
	public SpaceData SpaceData { get; init; }
	public SpaceView SpaceView { get; init; }
}
