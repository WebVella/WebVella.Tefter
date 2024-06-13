namespace WebVella.Tefter.Demo.Components;
public partial class WvSpace : WvBaseComponent
{
	private SpaceView _spaceView = null;
	private IQueryable<DataRow> _data = Enumerable.Empty<DataRow>().AsQueryable();
	private IEnumerable<DataRow> _selectedItems = Enumerable.Empty<DataRow>();
	private bool _isGridLoading = true;
	private bool _isMoreLoading = false;
	private bool _allLoaded = false;
	private int _page = 1;
	private int _pageSize = WvConstants.PageSize;
	private RenderFragment options = builder =>
	{
		builder.OpenElement(0, "div");
		builder.AddContent(1, "Hello from RenderFragment!");
		builder.CloseElement();
	};
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			WvState.ActiveSpaceDataChanged -= onSpaceDataLocation;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			var meta = WvState.GetActiveSpaceMeta();
			_spaceView = meta.SpaceView;
			if (_spaceView is not null)
			{
				await loadDataAsync(1);
				WvState.ActiveSpaceDataChanged += onSpaceDataLocation;
			}
			_isGridLoading = false;
			await InvokeAsync(StateHasChanged);

		}
	}

	protected void onSpaceDataLocation(object sender, StateActiveSpaceDataChangedEventArgs e)
	{
		base.InvokeAsync(async () =>
		{
			if (e.SpaceView.Id == _spaceView.Id) return;

			_isGridLoading = true;
			await InvokeAsync(StateHasChanged);
			await loadDataAsync(1);
			_isGridLoading = false;
			await InvokeAsync(StateHasChanged);
		});

	}
	private async Task loadDataAsync(int page)
	{
		if (_spaceView is null) return;
		var alldata = WvService.GetSpaceViewData(_spaceView.Id).AsQueryable();
		if (page == 1)
		{
			_data = Enumerable.Empty<DataRow>().AsQueryable();
			_allLoaded = false;
		}
		var skip = RenderUtils.CalcSkip(_pageSize, page);
		var batch = alldata.Skip(RenderUtils.CalcSkip(_pageSize, page)).Take(_pageSize).AsQueryable();
		foreach (var item in batch)
		{
			addActions(item);
		}
		_data = _data.Concat(batch);
		_page = page;
		if (batch.Count() < _pageSize) _allLoaded = true;
	}

	private void addActions(DataRow row)
	{
		row.OnCellDataChange = (args) => { onCellDataChanged(row, args); };
	}

	private void onCellDataChanged(DataRow row, (string, object) args)
	{

	}

	private async Task _loadMoreAsync()
	{
		_isMoreLoading = true;
		await InvokeAsync(StateHasChanged);
		await loadDataAsync(_page + 1);

		_isMoreLoading = false;
		await InvokeAsync(StateHasChanged);
	}

	private async Task _selectionChange(IEnumerable<DataRow> items)
	{
		_selectedItems = items;
		WvState.SetSelectedRows(items);
		await InvokeAsync(StateHasChanged);
	}

	private async Task _onRowClick(FluentDataGridRow<DataRow> row)
	{

	}
}