using Microsoft.AspNetCore.Http;

namespace WebVella.Tefter.UI.Components;
public partial class TucSortQueryCard : TfBaseComponent
{
	[Parameter]
	public string? Title { get; set; } = null;

	[Parameter]
	public List<TfSortQuery> Items { get; set; } = new();

	[Parameter]
	public EventCallback<List<TfSortQuery>> ItemsChanged { get; set; }

	[Parameter]
	public List<TfSpaceViewColumn> ViewColumns { get; set; } = new();

	private TfSortQuery? _selectedOption = null;
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
			_columnDict[column.QueryName] = !String.IsNullOrWhiteSpace(column.Title) ? column.Title : "no title";
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

	private async Task _addSortColumn(TfSortQuery? sort)
	{
		if (sort is null || String.IsNullOrWhiteSpace(sort.Name)) return;
		if (Items.Any(x => x.Name == sort.Name)) return;

		var items = Items.ToList();
		items.Add(new TfSortQuery { Name = sort.Name, Direction = sort.Direction });
		await ItemsChanged.InvokeAsync(items);
	}


	private async Task _deleteSortColumn(TfSortQuery sort)
	{
		var items = Items.Where(x => x.Name != sort.Name).ToList();
		await ItemsChanged.InvokeAsync(items);
	}

	private async Task _directionChanged(TfSortQuery? sort, TfSortDirection newDirection)
	{
		if (sort is null || sort.Direction == (int)newDirection) return;
		var items = Items.ToList();
		var sortIndex = items.IndexOf(sort);
		if (sortIndex < 0) return;
		items[sortIndex].Direction = (int)newDirection;
		await ItemsChanged.InvokeAsync(items);
	}

}
