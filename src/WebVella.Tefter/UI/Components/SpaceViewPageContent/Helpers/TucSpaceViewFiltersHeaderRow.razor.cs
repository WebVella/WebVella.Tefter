namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewFiltersHeaderRow : TfBaseComponent, IAsyncDisposable
{
	[Parameter] public List<TfDataProvider> AllDataProviders { get; set; } = new();
	[Parameter] public List<TfSharedColumn> AllSharedColumns { get; set; } = new();
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public List<TfSpaceViewColumn> SpaceViewColumns { get; set; } = new();

	[CascadingParameter(Name = "TucSpaceViewPageContent")]
	public TucSpaceViewPageContent TucSpaceViewPageContent { get; set; } = null!;

	private Dictionary<string, TfDatabaseColumnType> _typeDict = new();
	private List<TfFilterQuery> _filters = new();
	private Dictionary<string, TfFilterQuery> _columnQueryFilterDict = new();
	private Dictionary<string, TfFilterBase> _columnBaseFilterDict = new();

	private readonly Dictionary<string, TfFilterQuery> _popoverQueryFilterDict = new();
	private readonly Dictionary<string, TfFilterBase> _popoverBaseFilterDict = new();
	private Guid? _openedFilterId = null;
	private bool _hasPinnedData = false;

	private IAsyncDisposable _spaceViewColumnUpdatedEventSubscriber = null!;
	private IAsyncDisposable _spaceViewUpdatedEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _spaceViewColumnUpdatedEventSubscriber.DisposeAsync();
		await _spaceViewUpdatedEventSubscriber.DisposeAsync();
	}

	protected override void OnInitialized() => _init();

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			Navigator.LocationChanged += On_NavigationStateChanged;
			_spaceViewColumnUpdatedEventSubscriber =
				await TfEventBus.SubscribeAsync<TfSpaceViewColumnUpdatedEventPayload>(
					handler: On_SpaceViewColumnUpdatedEventAsync);
			_spaceViewUpdatedEventSubscriber =
				await TfEventBus.SubscribeAsync<TfSpaceViewUpdatedEventPayload>(
					handler: On_SpaceViewUpdatedEventAsync);
		}
	}

	private void _init()
	{
		string? pinnedDataIdentity = Navigator.GetStringFromQuery(TfConstants.DataIdentityIdQueryName);
		string? pinnedDataIdentityValue = Navigator.GetStringFromQuery(TfConstants.DataIdentityValueQueryName);
		_hasPinnedData = !String.IsNullOrWhiteSpace(pinnedDataIdentity) &&
		                 !String.IsNullOrWhiteSpace(pinnedDataIdentityValue);

		_typeDict = SpaceViewColumns.ToQueryNameTypeDictionary(dataProviders: AllDataProviders,
			sharedColumns: AllSharedColumns);
		_filters = new();
		_columnBaseFilterDict = new();
		_columnQueryFilterDict = new();
		var stateFilters = TfAuthLayout.GetState().NavigationState.Filters ?? new();
		foreach (var column in SpaceViewColumns)
		{
			if (!_typeDict.ContainsKey(column.QueryName)) continue;

			var columnFilter = stateFilters.FirstOrDefault(x => x.QueryName == column.QueryName);
			var columnType = _typeDict[column.QueryName];
			if (columnFilter is not null)
			{
				_columnQueryFilterDict[column.QueryName] = columnFilter with { QueryName = columnFilter.QueryName };
			}
			else
			{
				_columnQueryFilterDict[column.QueryName] =
					new TfFilterQuery() { QueryName = column.QueryName, Value = null, Method = 0 };
			}

			_filters.Add(_columnQueryFilterDict[column.QueryName]);

			if (columnType == TfDatabaseColumnType.Boolean)
			{
				_columnBaseFilterDict[column.QueryName] =
					new TfFilterBoolean(_columnQueryFilterDict[column.QueryName].Method);
			}
			else if (columnType == TfDatabaseColumnType.DateOnly
			         || columnType == TfDatabaseColumnType.DateTime)
			{
				_columnBaseFilterDict[column.QueryName] = new TfFilterDateTime(
					_columnQueryFilterDict[column.QueryName].Method, _columnQueryFilterDict[column.QueryName].Value);
			}
			else if (columnType == TfDatabaseColumnType.Guid)
			{
				_columnBaseFilterDict[column.QueryName] =
					new TfFilterGuid(_columnQueryFilterDict[column.QueryName].Method);
			}
			else if (columnType == TfDatabaseColumnType.ShortInteger
			         || columnType == TfDatabaseColumnType.Integer
			         || columnType == TfDatabaseColumnType.LongInteger)
			{
				_columnBaseFilterDict[column.QueryName] =
					new TfFilterNumeric(_columnQueryFilterDict[column.QueryName].Method);
			}
			else if (columnType == TfDatabaseColumnType.Number)
			{
				_columnBaseFilterDict[column.QueryName] =
					new TfFilterNumeric(_columnQueryFilterDict[column.QueryName].Method);
			}
			else if (columnType == TfDatabaseColumnType.ShortText
			         || columnType == TfDatabaseColumnType.Text)
			{
				_columnBaseFilterDict[column.QueryName] =
					new TfFilterText(_columnQueryFilterDict[column.QueryName].Method);
			}

			_resetColumnPopover(column.QueryName);
		}
	}

	private void _resetColumnPopover(string queryName)
	{
		_popoverQueryFilterDict[queryName] = _columnQueryFilterDict[queryName] with
		{
			QueryName = _columnQueryFilterDict[queryName].QueryName
		};

		_popoverBaseFilterDict[queryName] = _columnBaseFilterDict[queryName] with
		{
			ColumnName = _columnBaseFilterDict[queryName].ColumnName
		};
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_init();
		StateHasChanged();
	}

	private async Task On_SpaceViewUpdatedEventAsync(string? key, TfSpaceViewUpdatedEventPayload? payload)
	{
		_init();
		await InvokeAsync(StateHasChanged);
	}

	private async Task On_SpaceViewColumnUpdatedEventAsync(string? key, TfSpaceViewColumnUpdatedEventPayload? payload)
	{
		_init();
		await InvokeAsync(StateHasChanged);
	}


	private async Task _valueChanged(string queryName, object? valueObj,
		bool processMethod = false, object? methodObj = null)
	{
		var updateObj = _filters.FirstOrDefault(x => x.QueryName == queryName);
		if (updateObj is null)
		{
			updateObj = new TfFilterQuery() { QueryName = queryName, Method = 0 };
			_filters.Add(updateObj);
		}

		var type = _typeDict[queryName];
		if (type == TfDatabaseColumnType.Boolean)
		{
			var baseFilter = (TfFilterBoolean)_columnBaseFilterDict[queryName];
			if (processMethod)
			{
				var value = (int)baseFilter.ComparisonMethod;
				if (methodObj is not null) value = (int)methodObj;
				updateObj.Method = value;
			}

			{
				var value = (Option<string>?)valueObj;
				baseFilter.ValueOptionChanged(value);
				updateObj.Value = baseFilter.Value;
			}
		}
		else if (type == TfDatabaseColumnType.DateOnly
		         || type == TfDatabaseColumnType.DateTime)
		{
			var baseFilter = (TfFilterDateTime)_columnBaseFilterDict[queryName];
			if (processMethod)
			{
				var value = (int)baseFilter.ComparisonMethod;
				if (methodObj is not null) value = (int)methodObj;
				updateObj.Method = value;
			}

			{
				baseFilter.ValueStringChanged(((string?)valueObj)?.Trim());
				updateObj.Value = baseFilter.Value;
			}
		}
		else if (type == TfDatabaseColumnType.Guid)
		{
			var baseFilter = (TfFilterGuid)_columnBaseFilterDict[queryName];
			if (processMethod)
			{
				var value = (int)baseFilter.ComparisonMethod;
				if (methodObj is not null) value = (int)methodObj;
				updateObj.Method = value;
			}

			{
				var value = (string?)valueObj;
				if (!String.IsNullOrWhiteSpace(value) && !Guid.TryParse(value, out Guid _))
					ToastService.ShowError(LOC("Invalid GUID value"));
				baseFilter.ValueStringChanged(value);
				updateObj.Value = baseFilter.Value;
			}
		}
		else if (type == TfDatabaseColumnType.ShortInteger
		         || type == TfDatabaseColumnType.Integer
		         || type == TfDatabaseColumnType.LongInteger)
		{
			var baseFilter = (TfFilterNumeric)_columnBaseFilterDict[queryName];
			if (processMethod)
			{
				var value = (int)baseFilter.ComparisonMethod;
				if (methodObj is not null) value = (int)methodObj;
				updateObj.Method = value;
			}

			{
				baseFilter.ValueChanged((decimal?)(long?)valueObj);
				updateObj.Value = baseFilter.Value;
			}
		}
		else if (type == TfDatabaseColumnType.Number)
		{
			var baseFilter = (TfFilterNumeric)_columnBaseFilterDict[queryName];
			if (processMethod)
			{
				var value = (int)baseFilter.ComparisonMethod;
				if (methodObj is not null) value = (int)methodObj;
				updateObj.Method = value;
			}

			{
				baseFilter.ValueChanged((decimal?)valueObj);
				updateObj.Value = baseFilter.Value;
			}
		}
		else if (type == TfDatabaseColumnType.ShortText
		         || type == TfDatabaseColumnType.Text)
		{
			var baseFilter = (TfFilterText)_columnBaseFilterDict[queryName];
			if (processMethod)
			{
				var value = (int)baseFilter.ComparisonMethod;
				if (methodObj is not null) value = (int)methodObj;
				updateObj.Method = value;
			}

			{
				baseFilter.ValueChanged(((string?)valueObj)?.Trim());
				updateObj.Value = baseFilter.Value;
			}
		}
		else throw new Exception("Unsupported TucFilterBase in _valueChanged");


		await TucSpaceViewPageContent.OnFilter(_filters.Where(x => !String.IsNullOrWhiteSpace(x.Value) || x.Method != 0)
			.ToList());
	}

	private void _popoverOpenChanged(bool opened, TfSpaceViewColumn column)
	{
		_openedFilterId = opened ? column.Id : null;
		if (!opened)
			_resetColumnPopover(column.QueryName);
	}

	private async Task _popoverSubmit(TfSpaceViewColumn column)
	{
		var popoverQueryFilter = _popoverQueryFilterDict[column.QueryName];
		await _valueChanged(column.QueryName, popoverQueryFilter.Value, true, popoverQueryFilter.Method);
		_openedFilterId = null;
	}

	private async Task _popoverClear(TfSpaceViewColumn column)
	{
		await _valueChanged(column.QueryName, null);
		_openedFilterId = null;
	}
}