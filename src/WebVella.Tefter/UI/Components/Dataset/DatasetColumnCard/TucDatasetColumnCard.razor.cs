using System.Linq;

namespace WebVella.Tefter.UI.Components;

public partial class TucDatasetColumnCard : TfBaseComponent
{
	[Inject] public ITfDatasetUIService TfDatasetUIService { get; set; } = default!;
	[Parameter]
	public string? Title { get; set; } = null;

	[Parameter]
	public TfDataset Dataset { get; set; } = default!;

	[Parameter]
	public EventCallback<List<TfDatasetColumn>> ItemsChanged { get; set; }

	[Parameter]
	public string NoItemsMessage { get; set; } = "This dataset will return all data provider columns. Select columns for limitation.";

	List<TfDatasetColumn> _allOptions = new();
	List<TfDatasetColumn> _options = new();
	private TfDatasetColumn? _selectedColumn = null;
	public bool _submitting = false;

	List<TfDatasetColumn> _items = default!;

	protected override void OnInitialized()
	{
		if (Dataset is null) throw new Exception("Dataset is required");
		_allOptions = TfDatasetUIService.GetDatasetColumnOptions(Dataset.Id);
		_initOptions();
	}

	protected override void OnParametersSet()
	{
		_initOptions();
	}

	void _initOptions()
	{
		_items = new List<TfDatasetColumn>();
		foreach (var column in Dataset.Columns)
		{
			var option = _allOptions.FirstOrDefault(x => x.ColumnName == column);
			if (option != null)
				_items.Add(option);
		}

		foreach (var identity in Dataset.Identities)
		{
			foreach (var column in identity.Columns)
			{
				var option = _allOptions.FirstOrDefault(x => x.DataIdentity == identity.DataIdentity && x.SourceColumnName == column);
				if (option != null)
					_items.Add(option);
			}
		}

		var current = _items.Select(x => x.ColumnName).ToList();
		_options = _allOptions.Where(x => !current.Contains(x.ColumnName)).ToList();

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
