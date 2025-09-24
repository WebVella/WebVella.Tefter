namespace WebVella.Tefter.UI.Components;
public partial class TucFilterCard : TfBaseComponent
{

	[Parameter]
	public string? Title { get; set; } = null;
	[Parameter]
	public TfDataProvider? DataProvider { get; set; } = null;
	[Parameter]
	public List<TfFilterBase> Items { get; set; } = new();
	[Parameter]
	public EventCallback<List<TfFilterBase>> ItemsChanged { get; set; }

	public List<string?> AllColumnOptions
	{
		get
		{
			if (DataProvider is null) return new List<string?>();
			return DataProvider.Columns.Select(x => x.DbName).ToList();
		}
	}

	internal string? _selectedColumn = null;
	public bool _submitting = false;

	private async Task _addColumnFilterHandler()
	{
		if (String.IsNullOrWhiteSpace(_selectedColumn)) return;
		await AddColumnFilter(_selectedColumn, null);
		//_selectedFilterColumn = null; //do not clear for convenience
	}

	public async Task AddColumnFilter(string dbColumn, Guid? parentId)
	{
		if (String.IsNullOrWhiteSpace(dbColumn)) return;
		if (DataProvider is null) return;
		var column = DataProvider.Columns.FirstOrDefault(x => x.DbName == dbColumn);
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
	}

	public async Task AddFilter(Type type, string dbName, Guid? parentId)
	{
		TfFilterBase? filter = null;
		if (type == typeof(TfFilterAnd)) filter = new TfFilterAnd() { ColumnName = dbName };
		else if (type == typeof(TfFilterOr)) filter = new TfFilterOr() { ColumnName = dbName };
		else if (type == typeof(TfFilterBoolean)) filter = new TfFilterBoolean(dbName,TfFilterBooleanComparisonMethod.IsTrue,null);
		else if (type == typeof(TfFilterDateTime)) filter = new TfFilterDateTime(dbName, TfFilterDateTimeComparisonMethod.Greater,null);
		else if (type == typeof(TfFilterGuid)) filter = new TfFilterGuid(dbName, TfFilterGuidComparisonMethod.Equal,null);
		else if (type == typeof(TfFilterNumeric)) filter = new TfFilterNumeric(dbName,TfFilterNumericComparisonMethod.Equal,null);
		else if (type == typeof(TfFilterText)) filter = new TfFilterText(dbName, TfFilterTextComparisonMethod.Equal,null);
		else throw new Exception("Filter type not supported");
		if (parentId is null)
		{
			Items.Add(filter);
		}
		else
		{
			TfFilterBase? parentFilter = null;
			foreach (var item in Items)
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

		await ItemsChanged.InvokeAsync(Items);
	}

	public async Task RemoveColumnFilter(Guid filterId)
	{
		TfFilterBase? filter = null;
		TfFilterBase? parentFilter = null;
		foreach (var item in Items)
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
			if (parentFilter is null) Items.Remove(filter);
			else if (parentFilter is TfFilterAnd) ((TfFilterAnd)parentFilter).Remove(filter);
			else if (parentFilter is TfFilterOr) ((TfFilterOr)parentFilter).Remove(filter);
			await InvokeAsync(StateHasChanged);
			await ItemsChanged.InvokeAsync(Items);
		}
	}

	public async Task UpdateColumnFilter(TfFilterBase input)
	{
		TfFilterBase? filter = null;
		TfFilterBase? parentFilter = null;
		foreach (var item in Items)
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
			await ItemsChanged.InvokeAsync(Items);
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
