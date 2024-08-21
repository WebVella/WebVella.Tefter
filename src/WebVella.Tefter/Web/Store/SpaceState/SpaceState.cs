namespace WebVella.Tefter.Web.Store.SpaceState;

[FeatureState]
public partial record SpaceState
{
	public bool IsBusy { get; init; } = true;
}
