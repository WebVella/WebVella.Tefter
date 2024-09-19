namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceViewFiltersDialog.TfSpaceViewFiltersDialog", "WebVella.Tefter")]
public partial class TfSpaceViewFiltersDialog : TfFormBaseComponent, IDialogContentComponent<bool>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Parameter] public bool Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }
	private TucDataProvider _dataProvider = null;
	private TucSpaceData _spaceData = null;
	private List<TucFilterBase> _items = new List<TucFilterBase>();
	private string _activeTab = "current";
	internal string _selectedFilterColumn = null;
	public bool _submitting = false;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (TfAppState.Value.SpaceViewFilters is not null)
			_items = JsonSerializer.Deserialize<List<TucFilterBase>>(JsonSerializer.Serialize(TfAppState.Value.SpaceViewFilters));
		_spaceData = TfAppState.Value.SpaceDataList.FirstOrDefault(x => x.Id == TfAppState.Value.SpaceView.SpaceDataId);

		if (_spaceData is not null)
		{
			_dataProvider = TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == _spaceData.DataProviderId);
		}
	}

	public void _addFilter(Type type, string dbName, Guid? parentId)
	{
		TucFilterBase filter = null;
		if (type == typeof(TucFilterAnd)) filter = new TucFilterAnd() { ColumnName = dbName };
		else if (type == typeof(TucFilterOr)) filter = new TucFilterOr() { ColumnName = dbName };
		else if (type == typeof(TucFilterBoolean)) filter = new TucFilterBoolean() { ColumnName = dbName };
		else if (type == typeof(TucFilterDateOnly)) filter = new TucFilterDateOnly() { ColumnName = dbName };
		else if (type == typeof(TucFilterDateTime)) filter = new TucFilterDateTime() { ColumnName = dbName };
		else if (type == typeof(TucFilterGuid)) filter = new TucFilterGuid() { ColumnName = dbName };
		else if (type == typeof(TucFilterNumeric)) filter = new TucFilterNumeric() { ColumnName = dbName };
		else if (type == typeof(TucFilterText)) filter = new TucFilterText() { ColumnName = dbName };
		else throw new Exception("Filter type not supported");

		if (parentId is null)
		{
			_items.Add(filter);
		}
		else
		{
			TucFilterBase parentFilter = null;
			foreach (var item in _items)
			{
				var (result, resultParent) = FindFilter(item, parentId.Value, null);
				if (result is not null)
				{
					parentFilter = result;
					break;
				}
			}
			if (parentFilter is not null)
			{
				if (parentFilter is TucFilterAnd) ((TucFilterAnd)parentFilter).Filters.Add(filter);
				if (parentFilter is TucFilterOr) ((TucFilterOr)parentFilter).Filters.Add(filter);
			}
		}
	}

	public void _addColumnFilter(string dbColumn, Guid? parentId)
	{
		if (String.IsNullOrWhiteSpace(dbColumn)) return;
		if (_dataProvider is null) return;
		var column = _dataProvider.ColumnsTotal.FirstOrDefault(x => x.DbName == dbColumn);
		if (column is null) return;

		switch (column.DbType.TypeValue)
		{
			case TucDatabaseColumnType.ShortInteger:
			case TucDatabaseColumnType.Integer:
			case TucDatabaseColumnType.LongInteger:
			case TucDatabaseColumnType.Number:
				{
					_addFilter(typeof(TucFilterNumeric), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Boolean:
				{
					_addFilter(typeof(TucFilterBoolean), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Date:
				{
					_addFilter(typeof(TucFilterDateOnly), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.DateTime:
				{
					_addFilter(typeof(TucFilterDateTime), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.ShortText:
			case TucDatabaseColumnType.Text:
				{
					_addFilter(typeof(TucFilterText), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Guid:
				{
					_addFilter(typeof(TucFilterGuid), dbColumn, parentId);
				}
				break;
			default: throw new Exception("Unsupported column data type");

		}
	}

	public void _removeColumnFilter(Guid filterId)
	{
		TucFilterBase filter = null;
		TucFilterBase parentFilter = null;
		foreach (var item in _items)
		{
			var (result, resultParent) = FindFilter(item, filterId, null);
			if (result is not null)
			{
				filter = result;
				parentFilter = resultParent;
				break;
			}
		}

		if (filter is not null)
		{
			if (parentFilter is null) _items.Remove(filter);
			else if (parentFilter is TucFilterAnd) ((TucFilterAnd)parentFilter).Filters.Remove(filter);
			else if (parentFilter is TucFilterOr) ((TucFilterOr)parentFilter).Filters.Remove(filter);
		}
	}

	public void _updateColumnFilter(TucFilterBase filter)
	{
		//should be updated
	}

	private void _addColumnFilterHandler()
	{
		if (String.IsNullOrWhiteSpace(_selectedFilterColumn)) return;
		_addColumnFilter(_selectedFilterColumn, null);
	}

	private (TucFilterBase, TucFilterBase) FindFilter(TucFilterBase filter, Guid matchId, TucFilterBase parent)
	{
		if (filter.Id == matchId) return (filter, parent);
		List<TucFilterBase> filters = new();
		if (filter is TucFilterAnd) filters = ((TucFilterAnd)filter).Filters;
		if (filter is TucFilterOr) filters = ((TucFilterOr)filter).Filters;
		foreach (var item in filters)
		{
			var (result, resultParent) = FindFilter(item, matchId, filter);
			if (result is not null) return (result, resultParent);
		}
		return (null, null);
	}


	private async Task _submit()
	{
		await Dialog.CloseAsync(_items);
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

}
