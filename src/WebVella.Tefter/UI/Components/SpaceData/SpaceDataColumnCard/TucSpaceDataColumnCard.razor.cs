//namespace WebVella.Tefter.UI.Components;

//public partial class TucSpaceDataColumnCard : TfBaseComponent
//{

//	[Parameter]
//	public string? Title { get; set; } = null;

//	[Parameter]
//	public List<TfDatasetColumn>? Options { get; set; } = null;

//	[Parameter]
//	public List<TfDatasetColumn> Items { get; set; } = new();
//	[Parameter]
//	public EventCallback<TfDatasetColumn> AddColumn { get; set; }

//	[Parameter]
//	public EventCallback<TfDatasetColumn> RemoveColumn { get; set; }

//	[Parameter]
//	public EventCallback AddAllColumns { get; set; }

//	[Parameter]
//	public string NoItemsMessage { get; set; } = "This dataset will return all data provider columns. Select columns for limitation.";

//	[Parameter]
//	public RenderFragment? NoItemsTemplate { get; set; }

//	internal List<TfDatasetColumn> _columnOptions
//	{
//		get
//		{
//			if (Items.Count == 0) return Options.ToList();
//			return Options.Where(x => !Items.Any(y => y.ColumnName == x.ColumnName)).ToList();
//		}
//	}

//	private TfDatasetColumn? _selectedColumn = null;
//	public bool _submitting = false;

//	private async Task _addColumn(TfDatasetColumn? column)
//	{
//		if (_submitting || column is null) return;

//		if (!Items.Contains(column))
//		{
//			Items.Add(column);
//			await AddColumn.InvokeAsync(column);
//		}
//		_submitting = false;
//	}

//	private async Task _addAllColumns()
//	{
//		if (_submitting) return;
//		await AddAllColumns.InvokeAsync();
//		_submitting = false;
//	}

//	private async Task _deleteColumn(TfDatasetColumn column)
//	{
//		if (_submitting) return;
//		if (!Items.Contains(column)) return;
//		//if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?")))
//		//	return;

//		Items.Remove(column);
//		await RemoveColumn.InvokeAsync(column);

//		_submitting = false;
//		await InvokeAsync(StateHasChanged);
//	}
//}
