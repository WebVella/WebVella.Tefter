namespace WebVella.Tefter.Web.Store;

public record SetSpacePagingAction : TfBaseAction
{
	public int Page { get; }
	public int PageSize { get; }

	internal SetSpacePagingAction(
		TfBaseComponent component,
		int page,
		int pageSize
		)
	{
		Component = component;
		Page = page;
		PageSize = pageSize;
	}
}
