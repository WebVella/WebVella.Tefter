namespace WebVella.Tefter.UI.Components;

public partial class TucFilterCard : TfBaseComponent
{
	[Parameter]
	public TfDataset Dataset { get; set; } = default!;
	[Parameter]
	public EventCallback<List<TfFilterBase>> ItemsChanged { get; set; }

	List<string> _allOptions = new();
	Dictionary<string, TfDatasetColumn> _columnDict = new();

	internal string? _selectedColumn = null;
	public bool _submitting = false;
	List<TfFilterBase> _items = default!;

	protected override void OnInitialized()
	{
		if (Dataset is null) throw new Exception("Dataset is required");

		foreach (TfDatasetColumn item in TfUIService.GetDatasetColumnOptions(Dataset.Id))
		{
			if (String.IsNullOrWhiteSpace(item.ColumnName)) continue;
			_allOptions.Add(item.ColumnName);
			_columnDict[item.ColumnName] = item;
		}
		_initOptions();
	}

	protected override void OnParametersSet()
	{
		_initOptions();
	}

	void _initOptions()
	{
		_items = Dataset.Filters.ToList();
	}

	public async Task AddColumnFilter(string dbColumn, Guid? parentId)
	{
		if(!_columnDict.ContainsKey(dbColumn)) return;
		var column = _columnDict[dbColumn];
		if (column is null) return;

		switch (column.DbType)
		{
			case TfDatabaseColumnType.ShortInteger:
			case TfDatabaseColumnType.Integer:
			case TfDatabaseColumnType.LongInteger:
			case TfDatabaseColumnType.Number:
				{
					await AddFilter(typeof(TfFilterNumeric), dbColumn, parentId);
				}
				break;
			case TfDatabaseColumnType.Boolean:
				{
					await AddFilter(typeof(TfFilterBoolean), dbColumn, parentId);
				}
				break;
			case TfDatabaseColumnType.DateOnly:
			case TfDatabaseColumnType.DateTime:
				{
					await AddFilter(typeof(TfFilterDateTime), dbColumn, parentId);
				}
				break;
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				{
					await AddFilter(typeof(TfFilterText), dbColumn, parentId);
				}
				break;
			case TfDatabaseColumnType.Guid:
				{
					await AddFilter(typeof(TfFilterGuid), dbColumn, parentId);
				}
				break;
			default: throw new Exception("Unsupported column data type");

		}

		_selectedColumn = null;
		await InvokeAsync(StateHasChanged);
	}

	public async Task AddFilter(Type type, string dbName, Guid? parentId)
	{
		TfFilterBase? filter = null;
		if (type == typeof(TfFilterAnd)) filter = new TfFilterAnd() { ColumnName = dbName };
		else if (type == typeof(TfFilterOr)) filter = new TfFilterOr() { ColumnName = dbName };
		else if (type == typeof(TfFilterBoolean)) filter = new TfFilterBoolean(dbName, TfFilterBooleanComparisonMethod.IsTrue, null);
		else if (type == typeof(TfFilterDateTime)) filter = new TfFilterDateTime(dbName, TfFilterDateTimeComparisonMethod.Greater, null);
		else if (type == typeof(TfFilterGuid)) filter = new TfFilterGuid(dbName, TfFilterGuidComparisonMethod.Equal, null);
		else if (type == typeof(TfFilterNumeric)) filter = new TfFilterNumeric(dbName, TfFilterNumericComparisonMethod.Equal, null);
		else if (type == typeof(TfFilterText)) filter = new TfFilterText(dbName, TfFilterTextComparisonMethod.Equal, null);
		else throw new Exception("Filter type not supported");
		if (parentId is null)
		{
			_items.Add(filter);
		}
		else
		{
			TfFilterBase? parentFilter = null;
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
				if (parentFilter is TfFilterAnd) ((TfFilterAnd)parentFilter).Add(filter);
				if (parentFilter is TfFilterOr) ((TfFilterOr)parentFilter).Add(filter);
			}
		}

		await ItemsChanged.InvokeAsync(_items);
	}

	public async Task RemoveColumnFilter(Guid filterId)
	{
		TfFilterBase? filter = null;
		TfFilterBase? parentFilter = null;
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
			else if (parentFilter is TfFilterAnd) ((TfFilterAnd)parentFilter).Remove(filter);
			else if (parentFilter is TfFilterOr) ((TfFilterOr)parentFilter).Remove(filter);
			await InvokeAsync(StateHasChanged);
			await ItemsChanged.InvokeAsync(_items);
		}
	}

	public async Task UpdateColumnFilter(TfFilterBase input)
	{
		TfFilterBase? filter = null;
		TfFilterBase? parentFilter = null;
		foreach (var item in _items)
		{
			var (result, resultParent) = FindFilter(item, input.Id, null);
			if (result is not null)
			{
				filter = result;
				parentFilter = resultParent;
				break;
			}
		}

		if (filter is not null)
		{
			filter = input;
			await InvokeAsync(StateHasChanged);
			await ItemsChanged.InvokeAsync(_items);
		}
	}

	private (TfFilterBase?, TfFilterBase?) FindFilter(TfFilterBase filter, Guid matchId, TfFilterBase? parent)
	{
		if (filter.Id == matchId) return (filter, parent);
		List<TfFilterBase> filters = new();
		if (filter is TfFilterAnd) filters = ((TfFilterAnd)filter).Filters.ToList();
		if (filter is TfFilterOr) filters = ((TfFilterOr)filter).Filters.ToList();
		foreach (var item in filters)
		{
			var (result, resultParent) = FindFilter(item, matchId, filter);
			if (result is not null) return (result, resultParent);
		}
		return (null, null);
	}
}
