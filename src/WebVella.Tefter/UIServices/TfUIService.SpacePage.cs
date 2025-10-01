namespace WebVella.Tefter.UIServices;

public partial interface ITfUIService
{
	//Events
	event EventHandler<TfSpacePage> SpacePageCreated;
	event EventHandler<TfSpacePage> SpacePageUpdated;

	event EventHandler<TfSpacePage> SpacePageDeleted;

	//Space Page
	TfSpacePage GetSpacePage(Guid pageId);
	List<TfSpacePage> GetAllSpacePages();
	List<TfSpacePage> GetSpacePages(Guid spaceId);
	TfSpacePage CreateSpacePage(TfSpacePage page);
	TfSpacePage UpdateSpacePage(TfSpacePage page);
	void DeleteSpacePage(TfSpacePage page);
	void MoveSpacePage(TfSpacePage page, bool isMoveUp);
	void CopySpacePage(Guid pageId);
	TfSpacePage? GetSpacePageBySpaceViewId(Guid spaceViewId);
}

public partial class TfUIService : ITfUIService
{
	#region << Events >>

	public event EventHandler<TfSpacePage> SpacePageCreated = null!;
	public event EventHandler<TfSpacePage> SpacePageUpdated = null!;
	public event EventHandler<TfSpacePage> SpacePageDeleted = null!;

	#endregion

	#region << Space Page >>

	public TfSpacePage GetSpacePage(Guid pageId)
		=> _tfService.GetSpacePage(pageId);

	public List<TfSpacePage> GetAllSpacePages()
		=> _tfService.GetAllSpacePages();

	public List<TfSpacePage> GetSpacePages(Guid spaceId)
		=> _tfService.GetSpacePages(spaceId);

	public TfSpacePage CreateSpacePage(TfSpacePage page)
	{
		var (pageId, pageList) = _tfService.CreateSpacePage(page);
		var newPage = GetSpacePage(page.Id);
		SpacePageCreated?.Invoke(this, newPage);
		return newPage;
	}

	public TfSpacePage UpdateSpacePage(TfSpacePage page)
	{
		var pageList = _tfService.UpdateSpacePage(page);
		var newPage = GetSpacePage(page.Id);
		SpacePageUpdated?.Invoke(this, newPage);
		return newPage;
	}

	public void DeleteSpacePage(TfSpacePage page)
	{
		var pageList = _tfService.DeleteSpacePage(page);
		SpacePageDeleted?.Invoke(this, page);
	}

	public void MoveSpacePage(TfSpacePage page, bool isMoveUp)
	{
		if (isMoveUp)
			page.Position--;
		else
			page.Position++;
		var pageList = _tfService.UpdateSpacePage(page);
		SpaceUpdated?.Invoke(this, GetSpace(page.SpaceId));
	}

	public void CopySpacePage(Guid pageId)
	{
		var (newNodeId, nodesList) = _tfService.CopySpacePage(pageId);
		SpaceUpdated?.Invoke(this, GetSpace(nodesList[0].SpaceId));
	}

	public TfSpacePage? GetSpacePageBySpaceViewId(Guid spaceViewId)
	{
		var allPages = GetAllSpacePages();
		return allPages.FirstOrDefault(x => x.ComponentOptionsJson.Contains(spaceViewId.ToString()));
	}

	#endregion
}