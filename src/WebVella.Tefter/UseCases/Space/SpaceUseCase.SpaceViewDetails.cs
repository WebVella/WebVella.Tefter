namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal int Page { get; set; } = 1;

	internal Task InitSpaceViewDetails()
	{
		return Task.CompletedTask;
	}

	internal Task<TfDataTable> IInitSpaceViewDetailsAfterRender(TfState state)
	{
		if(state.SpaceView is null || state.SpaceView.SpaceDataId is null) return null;
		var dt = GetSpaceViewData(
			spaceDataId:state.SpaceView.SpaceDataId.Value,
			additionalFilters:state.Filters,
			sortOverrides:state.Sorts,
			search:state.SearchQuery,
			page:state.Page,
			pageSize: state.PageSize
			);
		return Task.FromResult(dt);

	}
}