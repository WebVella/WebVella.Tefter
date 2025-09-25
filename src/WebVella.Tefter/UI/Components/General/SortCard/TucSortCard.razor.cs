namespace WebVella.Tefter.UI.Components;

public partial class TucSortCard : TfBaseComponent
{
	[Inject] public ITfDatasetUIService TfDatasetUIService { get; set; } = default!;
	[Parameter]
	public TfDataset Dataset { get; set; } = default!;

	[Parameter]
	public EventCallback<List<TfSort>> ItemsChanged { get; set; }

	List<string> _allOptions = new();
	Dictionary<string, TfDatasetColumn> _columnDict = new();
	List<string> _options = new();
	string _selectedColumn = default!;
	TfSortDirection _selectedDirection = TfSortDirection.ASC;
	bool _submitting = false;
	List<TfSort> _items = default!;

	protected override void OnInitialized()
	{
		if (Dataset is null) throw new Exception("Dataset is required");

		foreach (TfDatasetColumn item in TfDatasetUIService.GetDatasetColumnOptions(Dataset.Id))
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
		_items = Dataset.SortOrders.ToList();

		var current = _items.Select(x => x.ColumnName).ToList();
		_options = _allOptions.Where(x => !current.Contains(x)).ToList();

		if (_options.Count > 0)
			_selectedColumn = _options[0];
	}

	private async Task _addSortColumn()
	{
		if (String.IsNullOrWhiteSpace(_selectedColumn)) return;
		if (_items.Any(x => x.ColumnName == _selectedColumn))
			return;

		_items.Add(new TfSort { ColumnName = _selectedColumn, Direction = _selectedDirection });
		await ItemsChanged.InvokeAsync(_items);
		_submitting = false;
		if (_options.Count > 0)
			_selectedColumn = _options[0];
		await InvokeAsync(StateHasChanged);

	}


	private async Task _deleteSortColumn(TfSort sort)
	{
		if (_submitting) return;
		if (!_items.Any(x => x.ColumnName == sort.ColumnName))
			return;

		_items = _items.Where(x => x.ColumnName != sort.ColumnName).ToList();

		await ItemsChanged.InvokeAsync(_items);
		_submitting = false;
		if (_options.Count > 0)
			_selectedColumn = _options[0];
		await InvokeAsync(StateHasChanged);

	}

}
