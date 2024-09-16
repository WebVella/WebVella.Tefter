namespace WebVella.Tefter.Web.Store;

public record InitSpaceViewDetailsAction : TfBaseAction
{
	public int Page { get; }
	public int PageSize { get; }
	public string SearchQuery { get; }
	public List<TucFilterBase> Filters { get; }
	public List<TucSort> Sorts { get; }
	public List<Guid> SelectedDataRows { get; }
	public TfDataTable SpaceViewData { get; }

	internal InitSpaceViewDetailsAction(
		TfBaseComponent component,
		int page,
		int pageSize,
		string search,
		List<TucFilterBase> filters,
		List<TucSort> sorts,
		List<Guid> selectedDataRows,
		TfDataTable spaceViewData
		)
	{
		Component = component;
		Page = page;
		PageSize = pageSize;
		SearchQuery = search;
		Filters = filters;
		Sorts = sorts;
		SelectedDataRows = selectedDataRows;
		SpaceViewData = spaceViewData;
	}
}
