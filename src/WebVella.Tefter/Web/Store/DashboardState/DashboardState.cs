namespace WebVella.Tefter.Web.Store.DashboardState;

[FeatureState]
public partial record DashboardState
{
	public bool IsBusy { get; init; } = true;
}
