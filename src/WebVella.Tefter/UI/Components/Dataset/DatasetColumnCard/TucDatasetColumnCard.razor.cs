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
		_options = AllOptions.Where(x => !current.Contains(x.ColumnName)).ToList();

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

		_submitting = true;
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);

		foreach (var option in _options)
		{
			if (_items.Any(x => x.ColumnName == option.ColumnName)) continue;
			_items.Add(option);
		}
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
