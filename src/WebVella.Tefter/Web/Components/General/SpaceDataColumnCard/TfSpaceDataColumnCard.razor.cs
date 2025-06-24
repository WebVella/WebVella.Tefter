namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.SpaceDataColumnCard.TfSpaceDataColumnCard", "WebVella.Tefter")]
public partial class TfSpaceDataColumnCard : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }

	[Parameter]
	public string Title { get; set; } = null;

	[Parameter]
	public List<TucSpaceDataColumn> Options { get; set; } = null;

	[Parameter]
	public List<TucSpaceDataColumn> Items { get; set; } = new();
	[Parameter]
	public EventCallback<TucSpaceDataColumn> AddColumn { get; set; }

	[Parameter]
	public EventCallback<TucSpaceDataColumn> RemoveColumn { get; set; }

	[Parameter]
	public string NoItemsMessage { get; set; } = "This dataset will return all data provider columns. Select columns for limitation.";

	[Parameter]
	public RenderFragment NoItemsTemplate { get; set; }

	internal List<TucSpaceDataColumn> _columnOptions
	{
		get
		{
			if (Items.Count == 0) return Options;
			return Options.Where(x => !Items.Contains(x)).ToList();
		}
	}

	private TucSpaceDataColumn _selectedColumn = null;
	public bool _submitting = false;

	private async Task _addColumn()
	{
		if (_submitting) return;

		if (_selectedColumn is null) return;
		if (Items.Contains(_selectedColumn)) return;

		Items.Add(_selectedColumn);

		await AddColumn.InvokeAsync(_selectedColumn);

		_submitting = false;
		_selectedColumn = null;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _deleteColumn(TucSpaceDataColumn column)
	{
		if (_submitting) return;
		if (!Items.Contains(column)) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?")))
			return;

		Items.Remove(column);
		await RemoveColumn.InvokeAsync(column);

		_submitting = false;
		await InvokeAsync(StateHasChanged);
	}
}
