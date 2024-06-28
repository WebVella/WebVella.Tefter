namespace WebVella.Tefter.Web.Store.SystemState;

[FeatureState]
public record SystemState
{
	public bool IsBusy { get; init; }
	public List<Role> Roles { get; init; } = new ();
	public List<DatabaseColumnTypeInfo> DataProviderColumnTypes { get; init; } = new ();
}
