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
		var roleSM = _tfService.CreateSpace(space);
		SpaceCreated?.Invoke(this, space);
		return space;
	}
	public TfSpace UpdateSpace(TfSpace space)
	{
		var roleSM = _tfService.UpdateSpace(space);
		SpaceUpdated?.Invoke(this, roleSM);
		return roleSM;
	}
	public void DeleteSpace(Guid spaceId)
	{
		var space = _tfService.GetSpace(spaceId);
		_tfService.DeleteSpace(spaceId);
		SpaceDeleted?.Invoke(this, space);
	}
	#endregion
}
