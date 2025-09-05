namespace WebVella.Tefter.UI.Components;
public partial class TucQuerySortCard : TfBaseComponent
{
	[Parameter]
	public string? Title { get; set; } = null;

	[Parameter]
	public List<TfSortQuery> Items { get; set; } = new();

	[Parameter]
	public EventCallback<List<TfSortQuery>> ItemsChanged { get; set; }

	[Parameter]
	public List<TfSpaceViewColumn> ViewColumns { get; set; } = new();

	private TfSortQuery _selectedOption = new();
	private List<TfSortQuery> _allOptions = new();
	private List<TfSortQuery> _options = new();
	private Dictionary<string, string> _columnDict = new();

	protected override void OnInitialized()
	{
		if(ViewColumns is null) throw new Exception("ViewColumns is required");
		_allOptions = new();
		foreach (var column in ViewColumns)
		{
			_allOptions.Add(new TfSortQuery { Name = column.QueryName });
			_columnDict[column.QueryName] = column.Title;
		}
		_regenOptionsAsync();

	}

	protected override void OnParametersSet()
	{
		_regenOptionsAsync();
	}

	private void _regenOptionsAsync()
	{
		_options = new();
		foreach (var option in _allOptions)
		{
			if (Items.Any(x => x.Name == option.Name)) continue;
			_options.Add(option);
		}
	}

	private async Task _addSortColumn()
	{
		if (String.IsNullOrWhiteSpace(_selectedOption.Name)) return;
		if (Items.Any(x => x.Name == _selectedOption.Name))
		{
			_selectedOption = new();
			return;
		}

		var items = Items.ToList();
		items.Add(new TfSortQuery { Name = _selectedOption.Name, Direction = _selectedOption.Direction });
		await ItemsChanged.InvokeAsync(items);
	}


	private async Task _deleteSortColumn(TfSortQuery sort)
	{
		if (!Items.Any(x => x.Name == _selectedOption.Name))
		{
			_selectedOption = new();
			return;
		}

		var items = Items.Where(x => x.Name != sort.Name).ToList();
		await ItemsChanged.InvokeAsync(items);
	}

}
