namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewQuickView : TfBaseComponent
{
	#region << Init >>

	[Parameter] public TfSpacePageAddonContext? Context { get; set; } = null;


	private string _tableId = "space-view-table";
	private bool _isDataLoading = true;
	private ReadOnlyDictionary<Guid, ITfSpaceViewColumnTypeAddon> _columnTypeMetaDict = null!;
	private TfSpace? _space = null;
	private TfSpacePage? _spacePage = null;
	private TfSpaceView? _spaceView = null;
	private List<TfSpaceViewColumn> _spaceViewColumns = new();
	private TfDataset? _dataset = null;
	private TfDataProvider? _dataProvider = null;
	private TfDataTable? _data = null;
	private Dictionary<string, object> _contextViewData = new();
	private Dictionary<Guid, Dictionary<Guid, TfSpaceViewColumnBase>> _regionContextDict = new();
	private Dictionary<Guid, TfSpaceViewRowPresentationMeta> _rowMeta = new();
	private Dictionary<Guid, TfSpaceViewColumnPresentationMeta> _columnsMeta = new();
	private Dictionary<string, string> _queryNameToColumnNameDict = new();
	private Dictionary<Guid, Dictionary<string, Tuple<TfColor?, TfColor?>>> _rowColoringCacheDictionary = new();
	#endregion

	#region << Lifecycle >>
	protected override async Task OnInitializedAsync()
	{
		_columnTypeMetaDict = TfMetaService.GetSpaceViewColumnTypeDictionary();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await _init();
			_isDataLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _init()
	{

		_data = null;
		_regionContextDict = new();
		_spacePage = Context!.SpacePage;
		// if (_spacePage is null || _spacePage.SpaceId != _navState.SpaceId.Value)
		// 	return;
		if (_spacePage is null)
			throw new Exception("Space page required");
		_space = Context!.Space;
		if (_space is null)
			throw new Exception("Space required");
		if (_spacePage.Type != TfSpacePageType.Page &&
			_spacePage.ComponentType.FullName != typeof(TucSpaceViewSpacePageAddon).FullName)
			throw new Exception("Component not matched");
		var options = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(_spacePage.ComponentOptionsJson);
		if (options is null || options.SpaceViewId is null)
			throw new Exception("Component options required");
		_spaceView = TfService.GetSpaceView(options.SpaceViewId.Value);
		if (_spaceView is null)
			throw new Exception("SpaceView not found");
		_spaceViewColumns = TfService.GetSpaceViewColumnsList(_spaceView.Id);
		_dataset = TfService.GetDataset(_spaceView.DatasetId);
		if (_dataset is null)
			return;
		_dataProvider = TfService.GetDataProvider(_dataset.DataProviderId);
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);
		_data = TfService.QueryDataProvider(_dataProvider,
			sorts:_dataset.SortOrders,
			relIdentityInfo: Context.RelDataIdentityQueryInfo);
		var state = TfAuthLayout.GetState();
		var meta = new SpaceViewMetaUtility().GenerateMeta(
			serviceProvider: ServiceProvider,
			tfService: TfService,
			currentUser: state.User,
			navState: state.NavigationState,
			data: _data,
			spaceView: _spaceView,
			preset: null,
			spaceViewColumns: _spaceViewColumns,
			selectedDataRows: new List<Guid>(),
			editAll: false,
			editedDataRows: new List<Guid>(),
			onRowChanged: EventCallback.Factory.Create<TfSpaceViewColumnDataChange>(this, () => { })
			);

		_rowMeta = meta.RowMeta;
		_columnsMeta = meta.ColumnsMeta;
		_regionContextDict = meta.RegionContextDict;
		_queryNameToColumnNameDict = meta.QueryNameToColumnNameDict;
		_rowColoringCacheDictionary = meta.RowColoringCacheDictionary;

	}

	private string? _getSafeColumnMetaString(Guid columnId, string propName)
	{
		if (!_columnsMeta.ContainsKey(columnId)) return String.Empty;
		object value = _columnsMeta[columnId].GetPropertyByName(propName) ?? String.Empty;
		return value.ToString();
	}

	private string? _getSafeRegionContextString(Guid rowId, Guid columnId, string propName)
	{
		if (!_regionContextDict.ContainsKey(rowId)) return String.Empty;
		if (!_regionContextDict[rowId].ContainsKey(columnId)) return String.Empty;
		object value = _regionContextDict[rowId][columnId].GetPropertyByName(propName) ?? String.Empty;
		return value.ToString();
	}

	#endregion
}