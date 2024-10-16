namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.FilterCard.TfFilterCard", "WebVella.Tefter")]
public partial class TfFilterCard : TfBaseComponent
{

	[Parameter]
	public TucDataProvider DataProvider { get; set; } = null;
	[Parameter]
	public List<TucFilterBase> Items { get; set; } = new();
	[Parameter]
	public EventCallback<List<TucFilterBase>> ItemsChanged { get; set; }

	public List<string> AllColumnOptions
	{
		get
		{
			if (DataProvider is null) return new List<string>();
			return DataProvider.ColumnsPublic.Select(x => x.DbName).ToList();
		}
	}

	internal string _selectedColumn = null;
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
		var column = DataProvider.ColumnsPublic.FirstOrDefault(x => x.DbName == dbColumn);
		if (column is null) return;

		switch (column.DbType.TypeValue)
		{
			case TucDatabaseColumnType.ShortInteger:
			case TucDatabaseColumnType.Integer:
			case TucDatabaseColumnType.LongInteger:
			case TucDatabaseColumnType.Number:
				{
					await AddFilter(typeof(TucFilterNumeric), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Boolean:
				{
					await AddFilter(typeof(TucFilterBoolean), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Date:
				{
					await AddFilter(typeof(TucFilterDateOnly), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.DateTime:
				{
					await AddFilter(typeof(TucFilterDateTime), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.ShortText:
			case TucDatabaseColumnType.Text:
				{
					await AddFilter(typeof(TucFilterText), dbColumn, parentId);
				}
				break;
			case TucDatabaseColumnType.Guid:
				{
					await AddFilter(typeof(TucFilterGuid), dbColumn, parentId);
				}
				break;
			default: throw new Exception("Unsupported column data type");

		}
	}

	public async Task AddFilter(Type type, string dbName, Guid? parentId)
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
			Items.Add(filter);
		}
		else
		{
			TucFilterBase parentFilter = null;
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
				if (parentFilter is TucFilterAnd) ((TucFilterAnd)parentFilter).Filters.Add(filter);
				if (parentFilter is TucFilterOr) ((TucFilterOr)parentFilter).Filters.Add(filter);
			}
		}

		await ItemsChanged.InvokeAsync(Items);
	}

	public async Task RemoveColumnFilter(Guid filterId)
	{
		TucFilterBase filter = null;
		TucFilterBase parentFilter = null;
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
			else if (parentFilter is TucFilterAnd) ((TucFilterAnd)parentFilter).Filters.Remove(filter);
			else if (parentFilter is TucFilterOr) ((TucFilterOr)parentFilter).Filters.Remove(filter);
			await InvokeAsync(StateHasChanged);
			await ItemsChanged.InvokeAsync(Items);
		}
	}

	public async Task UpdateColumnFilter(TucFilterBase input)
	{
		TucFilterBase filter = null;
		TucFilterBase parentFilter = null;
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
}
