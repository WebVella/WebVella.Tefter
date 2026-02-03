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

	private IAsyncDisposable? _spaceViewColumnUpdatedEventSubscriber = null;
	private IAsyncDisposable? _spaceViewUpdatedEventSubscriber = null;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		if (_spaceViewColumnUpdatedEventSubscriber is not null)
			await _spaceViewColumnUpdatedEventSubscriber.DisposeAsync();
		if (_spaceViewUpdatedEventSubscriber is not null)
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
					handler: On_SpaceViewColumnUpdatedEventAsync,
					matchKey: (_) => true);
			_spaceViewUpdatedEventSubscriber =
				await TfEventBus.SubscribeAsync<TfSpaceViewUpdatedEventPayload>(
					handler: On_SpaceViewUpdatedEventAsync,
					matchKey: (_) => true);
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
			if (column.Settings.FilterPresentation == TfSpaceViewColumnSettingsFilterPresentation.Hidden) continue;
			if (!_typeDict.ContainsKey(column.QueryName)) continue;

			var columnFilter = stateFilters.FirstOrDefault(x => x.QueryName == column.QueryName);
			var columnType = _typeDict[column.QueryName];
			if (columnFilter is not null)
			{
				_columnQueryFilterDict[column.QueryName] = columnFilter with { QueryName = columnFilter.QueryName };
			}
			else if (!String.IsNullOrWhiteSpace(column.Settings.DefaultComparisonMethodDescription))
			{
				var dbType = _typeDict[column.QueryName];
				int method = 0;
				switch (dbType)
				{
					case TfDatabaseColumnType.ShortInteger:
					case TfDatabaseColumnType.Integer:
					case TfDatabaseColumnType.LongInteger:
					case TfDatabaseColumnType.Number:
						{
							TfFilterNumericComparisonMethod? methodEnum = Enum
								.GetValues<TfFilterNumericComparisonMethod>().FirstOrDefault(x =>
									x.ToDescriptionString() == column.Settings.DefaultComparisonMethodDescription);
							if (methodEnum is not null)
								method = (int)methodEnum;
						}
						break;
					case TfDatabaseColumnType.Boolean:
						{
							TfFilterBooleanComparisonMethod? methodEnum = Enum
								.GetValues<TfFilterBooleanComparisonMethod>().FirstOrDefault(x =>
									x.ToDescriptionString() == column.Settings.DefaultComparisonMethodDescription);
							if (methodEnum is not null)
								method = (int)methodEnum;
						}
						break;
					case TfDatabaseColumnType.DateOnly:
					case TfDatabaseColumnType.DateTime:
						{
							TfFilterDateTimeComparisonMethod? methodEnum = Enum
								.GetValues<TfFilterDateTimeComparisonMethod>().FirstOrDefault(x =>
									x.ToDescriptionString() == column.Settings.DefaultComparisonMethodDescription);
							if (methodEnum is not null)
								method = (int)methodEnum;
						}
						break;
					case TfDatabaseColumnType.ShortText:
					case TfDatabaseColumnType.Text:
						{
							TfFilterTextComparisonMethod? methodEnum = Enum
								.GetValues<TfFilterTextComparisonMethod>().FirstOrDefault(x =>
									x.ToDescriptionString() == column.Settings.DefaultComparisonMethodDescription);
							if (methodEnum is not null)
								method = (int)methodEnum;
						}
						break;
					case TfDatabaseColumnType.Guid:
						{
							TfFilterGuidComparisonMethod? methodEnum = Enum
								.GetValues<TfFilterGuidComparisonMethod>().FirstOrDefault(x =>
									x.ToDescriptionString() == column.Settings.DefaultComparisonMethodDescription);
							if (methodEnum is not null)
								method = (int)methodEnum;
						}
						break;
				}

				_columnQueryFilterDict[column.QueryName] =
					new TfFilterQuery() { QueryName = column.QueryName, Value = null, Method = method };
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
		if (payload is null) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
		{
			_init();
			await InvokeAsync(StateHasChanged);
		}
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}


	private async Task On_SpaceViewColumnUpdatedEventAsync(string? key, TfSpaceViewColumnUpdatedEventPayload? payload)
	{
		if (payload is null) return;
		if (SpaceViewColumns.All(x => x.Id != payload.ColumnId)) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
		{
			_init();
			await InvokeAsync(StateHasChanged);
		}
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}


	private async Task _valueChanged(string queryName, string? valueJson,
		bool processMethod = false, object? methodObj = null)
	{
		var updateObj = _filters.FirstOrDefault(x => x.QueryName == queryName);
		if (updateObj is null)
		{
			updateObj = new TfFilterQuery() { QueryName = queryName, Method = 0 };
			_filters.Add(updateObj);
		}

		//If no method is defined check for default in the view column settings
		var column = SpaceViewColumns.FirstOrDefault(x => x.QueryName == queryName);
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
			else if (column is not null)
			{
				TfFilterBooleanComparisonMethod? method = Enum.GetValues<TfFilterBooleanComparisonMethod>()
					.FirstOrDefault(x =>
						x.ToDescriptionString() == column.Settings.DefaultComparisonMethodDescription);
				method ??= TfFilterBooleanComparisonMethod.Equal;
				updateObj.Method = (int)method;
			}

			{
				baseFilter.ValueStringChanged(valueJson);
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
			else if (column is not null)
			{
				TfFilterDateTimeComparisonMethod? method = Enum.GetValues<TfFilterDateTimeComparisonMethod>()
					.FirstOrDefault(x =>
						x.ToDescriptionString() == column.Settings.DefaultComparisonMethodDescription);
				method ??= TfFilterDateTimeComparisonMethod.Equal;
				updateObj.Method = (int)method;
			}

			{
				baseFilter.ValueStringChanged(((string?)valueJson)?.Trim());
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
			else if (column is not null)
			{
				TfFilterGuidComparisonMethod? method = Enum.GetValues<TfFilterGuidComparisonMethod>()
					.FirstOrDefault(x =>
						x.ToDescriptionString() == column.Settings.DefaultComparisonMethodDescription);
				method ??= TfFilterGuidComparisonMethod.Equal;
				updateObj.Method = (int)method;
			}

			{
				if (!String.IsNullOrWhiteSpace(valueJson) && !Guid.TryParse(valueJson, out Guid _))
					ToastService.ShowError(LOC("Invalid GUID value"));
				baseFilter.ValueStringChanged(valueJson);
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
			else if (column is not null)
			{
				TfFilterNumericComparisonMethod? method = Enum.GetValues<TfFilterNumericComparisonMethod>()
					.FirstOrDefault(x =>
						x.ToDescriptionString() == column.Settings.DefaultComparisonMethodDescription);
				method ??= TfFilterNumericComparisonMethod.Equal;
				updateObj.Method = (int)method;
			}

			{
				baseFilter.ValueStringChanged(valueJson);
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
			else if (column is not null)
			{
				TfFilterNumericComparisonMethod? method = Enum.GetValues<TfFilterNumericComparisonMethod>()
					.FirstOrDefault(x =>
						x.ToDescriptionString() == column.Settings.DefaultComparisonMethodDescription);
				method ??= TfFilterNumericComparisonMethod.Equal;
				updateObj.Method = (int)method;
			}

			{
				baseFilter.ValueStringChanged(valueJson);
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
			else if (column is not null)
			{
				TfFilterTextComparisonMethod? method = Enum.GetValues<TfFilterTextComparisonMethod>()
					.FirstOrDefault(x =>
						x.ToDescriptionString() == column.Settings.DefaultComparisonMethodDescription);
				method ??= TfFilterTextComparisonMethod.Equal;
				updateObj.Method = (int)method;
			}

			{
				baseFilter.ValueChanged(((string?)valueJson)?.Trim());
				updateObj.Value = baseFilter.Value;
			}
		}
		else throw new Exception("Unsupported TucFilterBase in _valueChanged");

		_cleanUnusedFilters();
		await TucSpaceViewPageContent.OnFilter(_filters.Where(x => !String.IsNullOrWhiteSpace(x.Value) || x.Method != 0)
			.ToList());
	}

	private void _cleanUnusedFilters()
	{
		var cleanFilters = new List<TfFilterQuery>();
		foreach (var filter in _filters)
		{
			if (!_typeDict.ContainsKey(filter.QueryName) || filter.Items.Count > 0)
			{
				cleanFilters.Add(filter);
				continue;
			}


			var type = _typeDict[filter.QueryName];
			var requiresValue = false;

			if (type == TfDatabaseColumnType.Boolean)
			{
				var baseFilter = (TfFilterBoolean)_columnBaseFilterDict[filter.QueryName];
				requiresValue = baseFilter.RequiresValue;
			}
			else if (type == TfDatabaseColumnType.DateOnly
			         || type == TfDatabaseColumnType.DateTime)
			{
				var baseFilter = (TfFilterDateTime)_columnBaseFilterDict[filter.QueryName];
				requiresValue = baseFilter.RequiresValue;
			}
			else if (type == TfDatabaseColumnType.Guid)
			{
				var baseFilter = (TfFilterGuid)_columnBaseFilterDict[filter.QueryName];
				requiresValue = baseFilter.RequiresValue;
			}
			else if (type == TfDatabaseColumnType.ShortInteger
			         || type == TfDatabaseColumnType.Integer
			         || type == TfDatabaseColumnType.LongInteger)
			{
				var baseFilter = (TfFilterNumeric)_columnBaseFilterDict[filter.QueryName];
				requiresValue = baseFilter.RequiresValue;
			}
			else if (type == TfDatabaseColumnType.Number)
			{
				var baseFilter = (TfFilterNumeric)_columnBaseFilterDict[filter.QueryName];
				requiresValue = baseFilter.RequiresValue;
			}
			else if (type == TfDatabaseColumnType.ShortText
			         || type == TfDatabaseColumnType.Text)
			{
				var baseFilter = (TfFilterText)_columnBaseFilterDict[filter.QueryName];
				requiresValue = baseFilter.RequiresValue;
			}

			if (requiresValue && String.IsNullOrWhiteSpace(filter.Value))
			{
				//skip this filter
			}
			else
			{
				cleanFilters.Add(filter);
			}
		}

		_filters = cleanFilters;
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