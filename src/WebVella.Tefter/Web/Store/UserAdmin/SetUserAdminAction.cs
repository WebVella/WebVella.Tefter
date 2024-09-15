namespace WebVella.Tefter.Web.Store;

public record SetUserAdminAction : TfBaseAction
{
	public TucUser ManagedUser { get; }

	public SetUserAdminAction(
		TfBaseComponent component,
		TucUser userDetails)
	{
		Component = component;
		ManagedUser = userDetails;
	}
}
