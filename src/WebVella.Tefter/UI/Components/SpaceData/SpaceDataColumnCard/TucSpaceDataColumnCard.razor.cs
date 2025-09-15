namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceDataColumnCard : TfBaseComponent
{

	[Parameter]
	public string? Title { get; set; } = null;

	[Parameter]
	public List<TfSpaceDataColumn>? Options { get; set; } = null;

	[Parameter]
	public List<TfSpaceDataColumn> Items { get; set; } = new();
	[Parameter]
	public EventCallback<TfSpaceDataColumn> AddColumn { get; set; }

	[Parameter]
	public EventCallback<TfSpaceDataColumn> RemoveColumn { get; set; }

	[Parameter]
	public EventCallback AddAllColumns { get; set; }

	[Parameter]
	public string NoItemsMessage { get; set; } = "This dataset will return all data provider columns. Select columns for limitation.";

	[Parameter]
	public RenderFragment? NoItemsTemplate { get; set; }

	internal List<TfSpaceDataColumn> _columnOptions
	{
		get
		{
			if (Items.Count == 0) return Options.ToList();
			return Options.Where(x => !Items.Any(y => y.ColumnName == x.ColumnName)).ToList();
		}
	}

	private TfSpaceDataColumn? _selectedColumn = null;
	public bool _submitting = false;

	private async Task _addColumn(TfSpaceDataColumn? column)
	{
		if (_submitting || column is null) return;

		if (!Items.Contains(column))
		{
			Items.Add(column);
			await AddColumn.InvokeAsync(column);
		}
		_submitting = false;
	}

	private async Task _addAllColumns()
	{
		if (_submitting) return;
		await AddAllColumns.InvokeAsync();
		_submitting = false;
	}

	private async Task _deleteColumn(TfSpaceDataColumn column)
	{
		if (_submitting) return;
		if (!Items.Contains(column)) return;
		//if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?")))
		//	return;

		Items.Remove(column);
		await RemoveColumn.InvokeAsync(column);

		_submitting = false;
		await InvokeAsync(StateHasChanged);
	}
}
