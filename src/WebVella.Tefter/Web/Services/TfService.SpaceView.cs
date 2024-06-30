namespace WebVella.Tefter.Web.Services;

public partial interface ITfService{
	Task<List<DemoDataRow>> GetSpaceViewData(Guid spaceViewId);
	Task<SpaceView> SetSpaceViewBookmarkState(Guid spaceViewId, bool isBookmarked);
	Task<List<SpaceView>> GetBookmaredByUserId(string search, Guid userId, int page, int pageSize);
}

public partial class TfService
{
	public async Task<List<DemoDataRow>> GetSpaceViewData(Guid spaceViewId)
	{
		await Task.Delay(300);
		return await dataBroker.GetDataAsync();
	}

	public async Task<SpaceView> SetSpaceViewBookmarkState(Guid spaceViewId, bool isBookmarked)
	{

		//SpaceView spaceView = SampleData.GetSpaceViewById(spaceViewId);
		//if (spaceView is null) throw new Exception("not found");
		//spaceView.IsBookmarked = isBookmarked;
		return await dataBroker.GetSpaceViewById(spaceViewId);
	}

	public async Task<List<SpaceView>> GetBookmaredByUserId(string search, Guid userId, int page, int pageSize)
	{
		return await dataBroker.GetBookmaredByUserId(search, userId, page, pageSize);
	}
}
