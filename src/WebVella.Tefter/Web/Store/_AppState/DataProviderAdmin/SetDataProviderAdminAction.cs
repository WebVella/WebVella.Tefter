namespace WebVella.Tefter.Web.Store;

public record SetDataProviderAdminAction : TfBaseAction
{
	public TucDataProvider Provider { get; }

	public SetDataProviderAdminAction(
		TfBaseComponent component,
		TucDataProvider provider
		)
	{
		Component = component;
		Provider = provider;
	}
}
