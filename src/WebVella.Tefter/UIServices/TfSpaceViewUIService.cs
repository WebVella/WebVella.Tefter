namespace WebVella.Tefter.UIServices;

public partial interface ITfSpaceViewUIService
{
	//Events
	event EventHandler<TfSpaceView> SpaceViewCreated;
	event EventHandler<TfSpaceView> SpaceViewUpdated;
	event EventHandler<TfSpaceView> SpaceViewDeleted;

	event EventHandler<List<TfSpaceViewColumn>> SpaceViewColumnsChanged;


	//Space View
	List<TfSpaceView> GetAllSpaceViews(string? search = null);
	List<TfSpaceView> GetSpaceViewsList(Guid spaceId, string? search = null);
	TfSpaceView GetSpaceView(Guid itemId);
	TfSpaceView CreateSpaceView(TfCreateSpaceViewExtended item);
	TfSpaceView UpdateSpaceView(TfCreateSpaceViewExtended item);
	void DeleteSpaceView(Guid itemId);

	//Space View Column
	TfSpaceViewColumnTypeAddonMeta GetSpaceViewColumnTypeById(Guid typeId);
	List<ITfSpaceViewColumnComponentAddon> GetSpaceViewColumnTypeSupportedComponents(Guid typeId);
	ITfSpaceViewColumnComponentAddon GetSpaceViewColumnComponentById(Guid componentId);
	TfSpaceViewColumn GetViewColumn(Guid columnId);
	List<TfSpaceViewColumn> GetViewColumns(Guid viewId);
	void CreateSpaceViewColumn(TfSpaceViewColumn column);
	void UpdateSpaceViewColumn(TfSpaceViewColumn column);
	void RemoveSpaceViewColumn(Guid columnId);
	void MoveSpaceViewColumn(
		Guid viewId,
		Guid columnId,
		bool isUp);


	//Presets
	void UpdateSpaceViewPresets(Guid viewId,List<TfSpaceViewPreset> presets);
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

	public event EventHandler<List<TfSpaceViewColumn>> SpaceViewColumnsChanged = default!;
	#endregion

	#region << Space View>>
	public List<TfSpaceView> GetAllSpaceViews(string? search = null)
		=> _tfService.GetAllSpaceViews(search);
	public List<TfSpaceView> GetSpaceViewsList(Guid spaceId, string? search = null)
		=> _tfService.GetSpaceViewsList(spaceId, search);
	public TfSpaceView GetSpaceView(Guid itemId) => _tfService.GetSpaceView(itemId);
	public TfSpaceView CreateSpaceView(TfCreateSpaceViewExtended submit)
	{
		var spaceView = _tfService.CreateSpaceView(submit, submit.DataSetType == TfSpaceViewDataSetType.New);
		SpaceViewCreated?.Invoke(this, spaceView);
		return spaceView;
	}
	public TfSpaceView UpdateSpaceView(TfCreateSpaceViewExtended submit)
	{
		if (submit.SpaceDataId is null)
			throw new Exception("SpaceDataId is required");

		var form = new TfSpaceView
		{
			Id = submit.Id,
			Name = submit.Name,
			Type = submit.Type,
			SpaceDataId = submit.SpaceDataId ?? Guid.Empty,
			SpaceId = submit.SpaceId,
			SettingsJson = JsonSerializer.Serialize(submit.Settings),
			Presets = submit.Presets,
			Position = submit.Position,
		};
		var spaceView = _tfService.UpdateSpaceView(form);
		SpaceViewUpdated?.Invoke(this, spaceView);
		return spaceView;
	}
	public void DeleteSpaceView(Guid itemId)
	{
		var spaceView = _tfService.GetSpaceView(itemId);
		_tfService.DeleteSpaceView(itemId);
		SpaceViewDeleted?.Invoke(this, spaceView);
	}
	#endregion

	#region << Space View Column>>
	public TfSpaceViewColumn GetViewColumn(
		Guid columnId)
		=> _tfService.GetSpaceViewColumn(columnId);

	public List<TfSpaceViewColumn> GetViewColumns(
		Guid viewId)
	=> _tfService.GetSpaceViewColumnsList(viewId) ?? new List<TfSpaceViewColumn>();

	public void CreateSpaceViewColumn(
			TfSpaceViewColumn column)
	{
		_tfService.CreateSpaceViewColumn(column);
		SpaceViewColumnsChanged?.Invoke(this, GetViewColumns(column.SpaceViewId));
	}

	public void UpdateSpaceViewColumn(
		TfSpaceViewColumn column)
	{
		_tfService.UpdateSpaceViewColumn(column);
		SpaceViewColumnsChanged?.Invoke(this, GetViewColumns(column.SpaceViewId));
	}

	public void RemoveSpaceViewColumn(
		Guid columnId)
	{
		if (columnId == Guid.Empty)
			throw new TfException("Column ID is not specified");

		var column = GetViewColumn(columnId);

		if (column is null)
			throw new TfException("Column is not found");

		_tfService.DeleteSpaceViewColumn(columnId);

		SpaceViewColumnsChanged?.Invoke(this, GetViewColumns(column.SpaceViewId));
	}

	public void MoveSpaceViewColumn(
		Guid viewId,
		Guid columnId,
		bool isUp)
	{
		if (columnId == Guid.Empty)
			throw new TfException("Column ID is not specified");
		;
		if (isUp)
			_tfService.MoveSpaceViewColumnUp(columnId);
		else
			_tfService.MoveSpaceViewColumnDown(columnId);

		SpaceViewColumnsChanged?.Invoke(this, GetViewColumns(viewId));
	}

	public TfSpaceViewColumnTypeAddonMeta GetSpaceViewColumnTypeById(Guid typeId)
		=> _metaService.GetSpaceViewColumnType(typeId);

	public List<ITfSpaceViewColumnComponentAddon> GetSpaceViewColumnTypeSupportedComponents(Guid typeId)
		=> _metaService.GetSpaceViewColumnTypeSupportedComponents(typeId);

	public ITfSpaceViewColumnComponentAddon GetSpaceViewColumnComponentById(Guid componentId)
		=> _metaService.GetSpaceViewColumnComponent(componentId);
	#endregion

	#region << Presets >>
	public void UpdateSpaceViewPresets(Guid viewId,List<TfSpaceViewPreset> presets){ 
		_tfService.UpdateSpaceViewPresets(viewId,presets);		
		var spaceView = _tfService.GetSpaceView(viewId);
		SpaceViewUpdated?.Invoke(this, spaceView);	
	}
	#endregion
}
