namespace WebVella.Tefter.UIServices;

public partial interface ITfSpaceViewUIService
{
	//Events
	event EventHandler<TfSpaceView> SpaceViewCreated;
	event EventHandler<TfSpaceView> SpaceViewUpdated;
	event EventHandler<TfSpaceView> SpaceViewDeleted;

	//Space View
	TfSpaceView GetSpaceView(Guid itemId);
	TfSpaceView CreateSpaceView(TfCreateSpaceViewExtended item);
	TfSpaceView UpdateSpaceView(TfCreateSpaceViewExtended item);
	void DeleteSpaceView(Guid itemId);
}
public partial class TfSpaceViewUIService : ITfSpaceViewUIService
{
	#region << Ctor >>
	private static readonly AsyncLock _asyncLock = new AsyncLock();
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfSpaceViewUIService> LOC;

	public TfSpaceViewUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfSpaceViewUIService>>() ?? default!;
	}
	#endregion

	#region << Events >>
	public event EventHandler<TfSpaceView> SpaceViewCreated = default!;
	public event EventHandler<TfSpaceView> SpaceViewUpdated = default!;
	public event EventHandler<TfSpaceView> SpaceViewDeleted = default!;
	#endregion

	#region << Space View>>
	public TfSpaceView GetSpaceView(Guid itemId) => _tfService.GetSpaceView(itemId);
	public TfSpaceView CreateSpaceView(TfCreateSpaceViewExtended submit)
	{
		//var spaceView = _tfService.CreateSpaceView(form);
		//SpaceViewCreated?.Invoke(this, spaceView);
		//return spaceView;
		return null;
	}
	public TfSpaceView UpdateSpaceView(TfCreateSpaceViewExtended submit)
	{
		//var form = new TfUpdateSpaceView
		//{
		//	Columns = submit.Columns,
		//	ViewProviderId = submit.ViewProviderId,
		//	Filters = submit.Filters,
		//	Id = submit.Id,
		//	Name = submit.Name,
		//	SortOrders = submit.SortOrders
		//};
		//var spaceView = _tfService.UpdateSpaceView(form);
		//SpaceViewUpdated?.Invoke(this, spaceView);
		//return spaceView;
		return null;
	}
	public void DeleteSpaceView(Guid itemId)
	{
		var spaceView = _tfService.GetSpaceView(itemId);
		_tfService.DeleteSpaceView(itemId);
		SpaceViewDeleted?.Invoke(this, spaceView);
	}
	#endregion
}
