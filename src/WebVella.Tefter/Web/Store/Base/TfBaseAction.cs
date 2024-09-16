namespace WebVella.Tefter.Web.Store;
public record TfBaseAction
{
	public TfBaseComponent Component { get; init;}
	public FluxorComponent StateComponent { get; init;}
}
