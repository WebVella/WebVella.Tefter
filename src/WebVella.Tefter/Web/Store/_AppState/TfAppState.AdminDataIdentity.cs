namespace WebVella.Tefter.Web.Store;

public partial record TfAppState
{
	public List<TucDataIdentity> AdminDataIdentities { get; init; } = new();
	public TucDataIdentity AdminDataIdentity { get; init; }
}
