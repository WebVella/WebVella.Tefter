namespace WebVella.Tefter.Demo.Services;

public partial interface IWvService
{
	List<DataRow> GetSpaceViewData(Guid spaceViewId);
	SpaceView SetSpaceViewBookmarkState(Guid spaceViewId, bool isBookmarked);
	List<SpaceView> GetBookmaredByUserId(string search, Guid userId, int page, int pageSize);
}

public partial class WvService : IWvService
{
	public List<DataRow> GetSpaceViewData(Guid spaceViewId)
	{ 
		return SampleData.GetData();	
	}

	public SpaceView SetSpaceViewBookmarkState(Guid spaceViewId,bool isBookmarked){ 
		
		SpaceView spaceView = SampleData.GetSpaceViewById(spaceViewId);
		if(spaceView is null) throw new Exception("not found");
		spaceView.IsBookmarked = isBookmarked;
		return SampleData.GetSpaceViewById(spaceViewId);
	}

	public List<SpaceView> GetBookmaredByUserId(string search, Guid userId,int page, int pageSize){ 
		return SampleData.GetBookmaredByUserId(search, userId,page,pageSize);
	}
}
