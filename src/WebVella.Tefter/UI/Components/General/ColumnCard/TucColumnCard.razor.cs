namespace WebVella.Tefter.UI.Components;
public partial class TucColumnCard : TfBaseComponent
{
	[Parameter]
	public string? Title { get; set; } = null;

	[Parameter]
	public TfDataProvider? DataProvider { get; set; } = null;

	[Parameter]
	public List<string> Options { get; set; } = new();

	[Parameter]
	public List<string> Items { get; set; } = new();
	[Parameter]
	public EventCallback<List<string>> ItemsChanged { get; set; }

	[Parameter]
	public string NoItemsMessage { get; set; } = "This dataset will return all data provider columns. Select columns for limitation.";

	[Parameter]
	public RenderFragment? NoItemsTemplate { get; set; }

	public List<string> AllColumnOptions
	{
		get
		{
			if (DataProvider is not null)
			{
				var result = new List<string>();
				result.AddRange(DataProvider.Columns.Select(x => x.DbName ?? String.Empty).ToList());
				return result.Order().ToList();
			}
			if (Options is not null) return Options.ToList();
			return new List<string>();
		}
	}
	internal List<string> _columnOptions
	{
		get
		{
			if (Items.Count == 0) return AllColumnOptions;
			return AllColumnOptions.Where(x => !Items.Contains(x)).ToList();
		}
	}

	private string? _selectedColumn = null;
	public bool _submitting = false;
	public Guid? _initedProviderId = null;

	private async Task _addColumn()
	{
		if (_submitting) return;

		if (String.IsNullOrWhiteSpace(_selectedColumn)) return;
		if (Items.Contains(_selectedColumn)) return;

		Items.Add(_selectedColumn);

		await ItemsChanged.InvokeAsync(Items);

		_submitting = false;
		_selectedColumn = null;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _deleteColumn(string column)
	{
		if (_submitting) return;
		if (!Items.Contains(column)) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?")))
			return;

		Items.Remove(column);
		await ItemsChanged.InvokeAsync(Items);

		_submitting = false;
		await InvokeAsync(StateHasChanged);
	}

	private TfDataProviderColumn? _getProviderColumnByName(string dbName)
	{
		return DataProvider?.Columns.FirstOrDefault(x => x.DbName == dbName);
	}
}
