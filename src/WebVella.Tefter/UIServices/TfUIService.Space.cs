namespace WebVella.Tefter.UIServices;

public partial interface ITfUIService
{
	//Events
	event EventHandler<TfSpace> SpaceCreated;
	event EventHandler<TfSpace> SpaceUpdated;
	event EventHandler<TfSpace> SpaceDeleted;

	//Space
	List<TfSpace> GetSpaces();
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
}
public partial class TfUIService : ITfUIService
{
	#region << Events >>
	public event EventHandler<TfSpace> SpaceCreated = null!;
	public event EventHandler<TfSpace> SpaceUpdated = null!;
	public event EventHandler<TfSpace> SpaceDeleted = null!;
	#endregion

	#region << Space >>

	public List<TfSpace> GetSpaces() => _tfService.GetSpacesList();
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
}
