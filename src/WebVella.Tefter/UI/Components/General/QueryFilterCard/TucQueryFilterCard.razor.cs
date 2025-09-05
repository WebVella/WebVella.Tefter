namespace WebVella.Tefter.UI.Components;
public partial class TucQueryFilterCard : TfBaseComponent
{

	[Parameter]
	public string? Title { get; set; } = null;
	[Parameter]
	public List<TfFilterQuery> Items { get; set; } = new();
	[Parameter]
	public EventCallback<List<TfFilterQuery>> ItemsChanged { get; set; }

	[Parameter]
	public List<TfSpaceViewColumn> ViewColumns { get; set; } = new();

	[Parameter]
	public List<TfDataProviderColumn> ProviderColumns { get; set; } = new();

	private TfFilterQuery _selectedOption = new();
	private List<TfFilterQuery> _allOptions = new();
	private Dictionary<string, string> _columnDict = new();
	private Dictionary<string, TfDatabaseColumnType> _typeDict = new();

	protected override void OnInitialized()
	{
		if (ViewColumns is null) throw new Exception("ViewColumns is required");
		_allOptions = new();
		foreach (var column in ViewColumns)
		{
			_allOptions.Add(new TfFilterQuery { Name = column.QueryName });
			_columnDict[column.QueryName] = column.Title;
			var columnName = column.GetColumnNameFromDataMapping();
			var providerColumn = ProviderColumns.FirstOrDefault(x => x.DbName == columnName);
			if (providerColumn is not null)
			{
				_typeDict[column.QueryName] = providerColumn.DbType;
			}
		}
	}

	private async Task _addColumnFilterHandler()
	{
		if (String.IsNullOrWhiteSpace(_selectedOption.Name)) return;
		await AddFilter(_selectedOption.Name, null);
	}

	public async Task AddFilter(string queryName, List<string>? parentPath)
	{
		TfFilterQuery filter = new() { Name = queryName };
		if (_typeDict.ContainsKey(queryName))
		{
			switch (_typeDict[queryName])
			{
				case TfDatabaseColumnType.ShortInteger:
				case TfDatabaseColumnType.Integer:
				case TfDatabaseColumnType.LongInteger:
				case TfDatabaseColumnType.Number:
					filter.Method = (int)TfFilterNumericComparisonMethod.Equal;
					break;
				case TfDatabaseColumnType.Boolean:
					filter.Method = (int)TfFilterBooleanComparisonMethod.IsTrue;
					break;
				case TfDatabaseColumnType.DateOnly:
				case TfDatabaseColumnType.DateTime:
					filter.Method = (int)TfFilterDateTimeComparisonMethod.Greater;
					break;
				case TfDatabaseColumnType.ShortText:
				case TfDatabaseColumnType.Text:
					filter.Method = (int)TfFilterTextComparisonMethod.Equal;
					break;
				case TfDatabaseColumnType.Guid:
					filter.Method = (int)TfFilterGuidComparisonMethod.Equal;
					break;
				default:
					break;

			}
		}
		var items = Items.ToList();
		if (parentPath is null)
		{
			items.Add(filter);
		}
		else
		{
			filter.Parent = items.GetNodeByPath(parentPath);
			if (filter.Parent is not null)
			{
				if (filter.Parent.Name == new TfFilterAnd().GetColumnName()
					|| filter.Parent.Name == new TfFilterOr().GetColumnName())
					filter.Parent.Items.Add(filter);
			}
		}

		await ItemsChanged.InvokeAsync(items);
	}

	public async Task RemoveFilter(TfFilterQuery filter)
	{
		if (filter is null) return;
		var items = Items.ToList();

		if (filter.Parent is null)
		{
			items.Remove(filter);
		}
		else
		{
			filter.Parent.Items.Remove(filter);
		}
		await ItemsChanged.InvokeAsync(items);
	}

	public async Task UpdateFilter(TfFilterQuery input)
	{
		var items = Items.ToList();
		var filter = items.GetNodeByPath(input.Path);
		if (filter is null) return;

		filter = input;
		await ItemsChanged.InvokeAsync(items);
	}
}
