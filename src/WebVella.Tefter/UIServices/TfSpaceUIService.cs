namespace WebVella.Tefter.UIServices;

public partial interface ITfSpaceUIService
{
	//Events
	event EventHandler<TfSpace> SpaceCreated;
	event EventHandler<TfSpace> SpaceUpdated;
	event EventHandler<TfSpace> SpaceDeleted;

	//Space
	TfSpace GetSpace(Guid spaceId);
	TfSpace CreateSpace(TfSpace space);
	TfSpace UpdateSpace(TfSpace space);
	void DeleteSpace(Guid spaceId);
	TfSpace SetSpacePrivacy(
			Guid spaceId,
			bool isPrivate);

	//Role
	TfSpace AddSpacesRole(TfSpace space, TfRole role);
	TfSpace RemoveSpacesRole(TfSpace space, TfRole role);

	//Space Page
	List<TfSpacePage> GetSpacePages(Guid spaceId);
	void CreateSpacePage(TfSpacePage page);
	void UpdateSpacePage(TfSpacePage page);
	void DeleteSpacePage(TfSpacePage page);
	void MoveSpacePage(TfSpacePage page, bool isMoveUp);
	void CopySpacePage(Guid pageId);
}
public partial class TfSpaceUIService : ITfSpaceUIService
{
	#region << Ctor >>
	private static readonly AsyncLock _asyncLock = new AsyncLock();
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfSpaceUIService> LOC;

	public TfSpaceUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfSpaceUIService>>() ?? default!;
	}
	#endregion

	#region << Events >>
	public event EventHandler<TfSpace> SpaceCreated = default!;
	public event EventHandler<TfSpace> SpaceUpdated = default!;
	public event EventHandler<TfSpace> SpaceDeleted = default!;
	#endregion

	#region << Space >>
	public TfSpace GetSpace(Guid spaceId) => _tfService.GetSpace(spaceId);
	public TfSpace CreateSpace(TfSpace space)
	{
		var item = _tfService.CreateSpace(space);
		SpaceCreated?.Invoke(this, item);
		return item;
	}
	public TfSpace UpdateSpace(TfSpace space)
	{
		var item = _tfService.UpdateSpace(space);
		SpaceUpdated?.Invoke(this, item);
		return item;
	}
	public void DeleteSpace(Guid spaceId)
	{
		var space = _tfService.GetSpace(spaceId);
		_tfService.DeleteSpace(spaceId);
		SpaceDeleted?.Invoke(this, space);
	}

	public TfSpace SetSpacePrivacy(
		Guid spaceId,
		bool isPrivate)
	{
		var space = _tfService.GetSpace(spaceId);
		if (space is null)
			throw new Exception("Space not found");
		space.IsPrivate = isPrivate;
		var result = _tfService.UpdateSpace(space);
		SpaceUpdated?.Invoke(this, space);
		return result;
	}

	#endregion

	#region << Role >>
	public TfSpace AddSpacesRole(TfSpace space, TfRole role)
	{
		_tfService.AddSpacesRole(new List<TfSpace> { space }, role);
		space = GetSpace(space.Id);
		SpaceUpdated?.Invoke(this, space);
		return space;
	}
	public TfSpace RemoveSpacesRole(TfSpace space, TfRole role)
	{
		_tfService.RemoveSpacesRole(new List<TfSpace> { space }, role);
		space = GetSpace(space.Id);
		SpaceUpdated?.Invoke(this, space);
		return space;
	}
	#endregion

	#region << Page >>
	public List<TfSpacePage> GetSpacePages(Guid spaceId)
		=> _tfService.GetSpacePages(spaceId);

	public void CreateSpacePage(TfSpacePage page)
	{
		var (pageId, pageList) = _tfService.CreateSpacePage(page);
		SpaceUpdated?.Invoke(this, GetSpace(page.SpaceId));
	}
	public void UpdateSpacePage(TfSpacePage page)
	{
		var pageList = _tfService.UpdateSpacePage(page);
		SpaceUpdated?.Invoke(this, GetSpace(page.SpaceId));
	}
	public void DeleteSpacePage(TfSpacePage page)
	{
		var pageList = _tfService.DeleteSpacePage(page);
		SpaceUpdated?.Invoke(this, GetSpace(page.SpaceId));
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

	#endregion
}
