namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal int Page { get; set; } = 1;
	internal int PageSize { get; set; } = TfConstants.PageSize;
	internal Task InitSpaceViewDetails()
	{
		return Task.CompletedTask;
	}

	internal Task IInitSpaceViewDetailsAfterRender(SpaceState state)
	{
		ViewColumns = GetViewColumns(state.SpaceView.Id);
		return Task.CompletedTask;
	}
}