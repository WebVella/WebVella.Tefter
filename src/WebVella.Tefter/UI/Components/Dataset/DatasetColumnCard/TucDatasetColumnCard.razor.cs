using System.Linq;

namespace WebVella.Tefter.UI.Components;

public partial class TucDatasetColumnCard : TfBaseComponent
{
	[Parameter]
	public List<TfDatasetColumn> Items { get; set; } = new();

	[Parameter]
	public List<TfDatasetColumn> AllOptions { get; set; } = new();

	[Parameter]
	public EventCallback<List<TfDatasetColumn>> ItemsChanged { get; set; }

	[Parameter]
	public string NoItemsMessage { get; set; } = "This dataset will return all data provider columns. Select columns for limitation.";

	List<TfDatasetColumn> _options = new();
	private TfDatasetColumn? _selectedColumn = null;
	public bool _submitting = false;

	List<TfDatasetColumn> _items = null!;
	private bool _hasAllColumns => _items.FindIndex(x => x.ColumnName == TfConstants.TF_DATASET_WILDCARD_COLUMN_SELECTOR) > -1;
	protected override void OnInitialized()
	{
		_initOptions();
	}

	protected override void OnParametersSet()
	{
		_initOptions();
	}

	void _initOptions()
	{
		_items = Items.ToList();
		var current = _items.Select(x => x.ColumnName).ToList();
		_options = AllOptions.Where(x => 
			!current.Contains(x.ColumnName)
			&& (!_hasAllColumns || x.SourceType != TfAuxDataSourceType.PrimaryDataProvider)).ToList();

		_submitting = false;
		_selectedColumn = null;
		StateHasChanged();
	}

	private async Task _addColumn(TfDatasetColumn? column)
	{
		if (column is null || _items.Any(x => x.ColumnName == column.ColumnName))
			return;

		if (_submitting) return;

		_submitting = true;
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);

		_items.Add(column);
		await ItemsChanged.InvokeAsync(_items);
	}

	private async Task _addAllColumns()
	{
		if (_options.Count == 0) return;
		if (_submitting) return;
		if (_hasAllColumns)
		{
			ToastService.ShowInfo(LOC("All columns selector is already added to this dataset"));
			return;
		}
		_submitting = true;
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);
		_items.Add(new TfDatasetColumn()
		{
			DataIdentity = null,
			ColumnName = TfConstants.TF_DATASET_WILDCARD_COLUMN_SELECTOR,
			SourceColumnName = null,
			SourceCode = null,
			SourceName = null,
			SourceType = TfAuxDataSourceType.PrimaryDataProvider,
			DbType = TfDatabaseColumnType.Text
		});
		// foreach (var option in _options)
		// {
		// 	if (_items.Any(x => x.ColumnName == option.ColumnName)) continue;
		// 	_items.Add(option);
		// }
		await ItemsChanged.InvokeAsync(_items);
	}

	private async Task _removeAllColumns()
	{
		if (_options.Count == 0) return;
		if (_submitting) return;
		if (!_hasAllColumns)
		{
			ToastService.ShowInfo(LOC("All columns selector is not present in this dataset"));
			return;
		}
		var allColumnsIndex = _items.FindIndex(x => x.ColumnName == TfConstants.TF_DATASET_WILDCARD_COLUMN_SELECTOR);
		_submitting = true;
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);
		_items.RemoveAt(allColumnsIndex);
		await ItemsChanged.InvokeAsync(_items);
	}

	private async Task _deleteColumn(TfDatasetColumn column)
	{
		if (_submitting) return;
		if (column is null || !_items.Any(x => x.ColumnName == column.ColumnName)) return;

		_items = _items.Where(x => x.ColumnName != column.ColumnName).ToList();
		await ItemsChanged.InvokeAsync(_items);
	}
}
