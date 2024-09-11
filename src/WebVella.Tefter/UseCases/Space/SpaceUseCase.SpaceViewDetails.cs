namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal int Page { get; set; } = 1;
	internal int PageSize { get; set; } = TfConstants.PageSize;
	internal Task InitSpaceViewDetails()
	{
		return Task.CompletedTask;
	}

	internal async Task<TfDataTable> IInitSpaceViewDetailsAfterRender(SpaceState state)
	{
		ViewColumns = GetViewColumns(state.SpaceView.Id);
		if(state.SpaceView is null || state.SpaceView.SpaceDataId is null) return null;
		return GetSpaceViewData(state.SpaceView.SpaceDataId.Value);
	}
}