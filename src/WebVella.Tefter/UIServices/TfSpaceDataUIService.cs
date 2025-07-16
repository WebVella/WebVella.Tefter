namespace WebVella.Tefter.UIServices;

public partial interface ITfSpaceDataUIService
{
	//Events
	event EventHandler<TfSpaceData> SpaceDataCreated;
	event EventHandler<TfSpaceData> SpaceDataUpdated;
	event EventHandler<TfSpaceData> SpaceDataDeleted;

	//Space Data
	TfSpaceData GetSpaceData(Guid spaceDataId);
	List<TfSpaceData> GetSpaceDataList(Guid spaceId);
	TfSpaceData CreateSpaceData(TfSpaceData item);
	TfSpaceData UpdateSpaceData(TfSpaceData item);
	void DeleteSpaceData(Guid itemId);
}
public partial class TfSpaceDataUIService : ITfSpaceDataUIService
{
	#region << Ctor >>
	private static readonly AsyncLock _asyncLock = new AsyncLock();
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfSpaceDataUIService> LOC;

	public TfSpaceDataUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfSpaceDataUIService>>() ?? default!;
	}
	#endregion

	#region << Events >>
	public event EventHandler<TfSpaceData> SpaceDataCreated = default!;
	public event EventHandler<TfSpaceData> SpaceDataUpdated = default!;
	public event EventHandler<TfSpaceData> SpaceDataDeleted = default!;
	#endregion

	#region << Space Data>>
	public TfSpaceData GetSpaceData(Guid itemId) => _tfService.GetSpaceData(itemId);
	public List<TfSpaceData> GetSpaceDataList(Guid spaceId) => _tfService.GetSpaceDataList(spaceId);
	public TfSpaceData CreateSpaceData(TfSpaceData submit)
	{
		var form = new TfCreateSpaceData
		{
			Columns = submit.Columns,
			DataProviderId = submit.DataProviderId,
			Filters = submit.Filters,
			Id = submit.Id,
			Name = submit.Name,
			SortOrders = submit.SortOrders,
			SpaceId = submit.SpaceId,
		};
		var spaceData = _tfService.CreateSpaceData(form);
		SpaceDataCreated?.Invoke(this, spaceData);
		return spaceData;
	}
	public TfSpaceData UpdateSpaceData(TfSpaceData submit)
	{
		var form = new TfUpdateSpaceData
		{
			Columns = submit.Columns,
			DataProviderId = submit.DataProviderId,
			Filters = submit.Filters,
			Id = submit.Id,
			Name = submit.Name,
			SortOrders = submit.SortOrders
		};
		var spaceData = _tfService.UpdateSpaceData(form);
		SpaceDataUpdated?.Invoke(this, spaceData);
		return spaceData;
	}
	public void DeleteSpaceData(Guid itemId)
	{
		var spaceData = _tfService.GetSpaceData(itemId);
		_tfService.DeleteSpaceData(itemId);
		SpaceDataDeleted?.Invoke(this, spaceData);
	}
	#endregion
}
