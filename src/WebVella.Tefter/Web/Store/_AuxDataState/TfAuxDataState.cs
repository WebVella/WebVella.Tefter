namespace WebVella.Tefter.Web.Store;
[FeatureState]
public partial record TfAuxDataState
{
	public Guid Hash { get; init; } = Guid.NewGuid();
	public Dictionary<string, object> Data { get; init; }
}