namespace WebVella.Tefter.Web.Services;

public partial interface ITfService{
	ValueTask<List<DataRow>> GetSpaceViewData(Guid spaceViewId);
	ValueTask<SpaceView> SetSpaceViewBookmarkState(Guid spaceViewId, bool isBookmarked);
	ValueTask<List<SpaceView>> GetBookmaredByUserId(string search, Guid userId, int page, int pageSize);
}

public partial class TfService
{
	public async ValueTask<List<DataRow>> GetSpaceViewData(Guid spaceViewId)
	{
		return await dataBroker.GetDataAsync();
	}

	public async ValueTask<SpaceView> SetSpaceViewBookmarkState(Guid spaceViewId, bool isBookmarked)
	{

		//SpaceView spaceView = SampleData.GetSpaceViewById(spaceViewId);
		//if (spaceView is null) throw new Exception("not found");
		//spaceView.IsBookmarked = isBookmarked;
		return await dataBroker.GetSpaceViewById(spaceViewId);
	}

	public async ValueTask<List<SpaceView>> GetBookmaredByUserId(string search, Guid userId, int page, int pageSize)
	{
		return await dataBroker.GetBookmaredByUserId(search, userId, page, pageSize);
	}
}
