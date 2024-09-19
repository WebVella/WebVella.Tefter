namespace WebVella.Tefter.Web.Store;


public partial record TfAppState
{
	public List<TucRole> UserRoles { get; init; } = new();
	public List<TucUser> AdminUsers { get; init; } = new();
	public TucUser AdminManagedUser { get; init; }
}
