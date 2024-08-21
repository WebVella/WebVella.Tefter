namespace WebVella.Tefter.Web.Store.FastAccessState;

[FeatureState]
public partial record FastAccessState
{
	public bool IsBusy { get; init; } = true;
}
