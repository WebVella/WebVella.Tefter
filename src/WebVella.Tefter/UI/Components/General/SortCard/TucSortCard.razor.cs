namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.SortCard.TfSortCard", "WebVella.Tefter")]
public partial class TucSortCard : TfBaseComponent
{
	[Parameter]
	public string? Title { get; set; } = null;
	[Parameter]
	public TfDataProvider? DataProvider { get; set; } = null;

	[Parameter]
	public List<TfSort> Items { get; set; } = new();
	[Parameter]
	public EventCallback<List<TfSort>> ItemsChanged { get; set; }
	public List<string> AllColumnOptions
	{
		get
		{
			if (DataProvider is null) return new List<string>();
			return DataProvider.Columns.Select(x => x.DbName).ToList();
		}
	}

	private string _selectedColumn = null;
	private TfSortDirection _selectedDirection = TfSortDirection.ASC;
	public bool _submitting = false;


	private async Task _addSortColumn()
	{
		if (String.IsNullOrWhiteSpace(_selectedColumn)) return;
		if(Items.Any(x=> x.ColumnName == _selectedColumn)){ 
			ToastService.ShowWarning(LOC("Column already added for sort"));
			return;
		}
		if (_submitting) return;
		Items.Add(new TfSort{ ColumnName = _selectedColumn, Direction = _selectedDirection });
		await ItemsChanged.InvokeAsync(Items);
		_submitting = false;
		_selectedColumn = null;
		await InvokeAsync(StateHasChanged);

	}


	private async Task _deleteSortColumn(TfSort sort)
	{
		if (_submitting) return;

		Items = Items.Where(x=> x.ColumnName != sort.ColumnName).ToList();

		await ItemsChanged.InvokeAsync(Items);
		_submitting = false;
		_selectedColumn = null;
		await InvokeAsync(StateHasChanged);

	}

}
