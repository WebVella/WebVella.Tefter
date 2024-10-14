namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.SortCard.TfSortCard", "WebVella.Tefter")]
public partial class TfSortCard : TfBaseComponent
{
	[Parameter]
	public TucDataProvider DataProvider { get; set; } = null;

	[Parameter]
	public List<TucSort> Items { get; set; } = new();
	[Parameter]
	public EventCallback<List<TucSort>> ItemsChanged { get; set; }
	public List<string> AllColumnOptions
	{
		get
		{
			if (DataProvider is null) return new List<string>();
			return DataProvider.ColumnsTotal.Select(x => x.DbName).ToList();
		}
	}

	private string _selectedColumn = null;
	private TucSortDirection _selectedDirection = TucSortDirection.ASC;
	public bool _submitting = false;


	private async Task _addSortColumn()
	{
		if(Items.Any(x=> x.DbName == _selectedColumn)){ 
			ToastService.ShowWarning(LOC("Column already added for sort"));
			return;
		}
		if (_submitting) return;
		Items.Add(new TucSort{ DbName = _selectedColumn, Direction = _selectedDirection });
		await ItemsChanged.InvokeAsync(Items);
		_submitting = false;
		_selectedColumn = null;
		await InvokeAsync(StateHasChanged);

	}


	private async Task _deleteSortColumn(TucSort sort)
	{
		if (_submitting) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this sort order removed?")))
			return;

		Items = Items.Where(x=> x.DbName != sort.DbName).ToList();

		await ItemsChanged.InvokeAsync(Items);
		_submitting = false;
		_selectedColumn = null;
		await InvokeAsync(StateHasChanged);

	}

}
