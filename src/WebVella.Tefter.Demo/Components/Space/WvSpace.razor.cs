namespace WebVella.Tefter.Demo.Components;
public partial class WvSpace : WvBaseComponent
{
	private Guid? _spaceViewId;
	private IQueryable<DataSource> _dataSource = Enumerable.Empty<DataSource>().AsQueryable();
	private IEnumerable<DataSource> _selectedItems = Enumerable.Empty<DataSource>();
	private bool _isGridLoading = true;
	private bool _isMoreLoading = false;
	private bool _allLoaded = false;
	private int _page = 1;
	private int _pageSize = WvConstants.PageSize;

	public override async ValueTask DisposeAsync()
	{
		WvState.ActiveSpaceDataChanged -= onSpaceDataLocation;
		await base.DisposeAsync();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			var meta = WvState.GetActiveSpaceMeta();
			_spaceViewId = meta.SpaceView?.Id;
			if (_spaceViewId is not null)
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
			if (e.SpaceView.Id == _spaceViewId) return;

			_isGridLoading = true;
			await InvokeAsync(StateHasChanged);
			await loadDataAsync(1);
			_isGridLoading = false;
			await InvokeAsync(StateHasChanged);
		});

	}
	private async Task loadDataAsync(int page)
	{
		if (_spaceViewId is null) return;
		await Task.Delay(500);
		var alldata = WvService.GetSpaceViewData(_spaceViewId.Value).AsQueryable();
		if (page == 1)
		{
			_dataSource = Enumerable.Empty<DataSource>().AsQueryable();
			_allLoaded = false;
		}
		var skip = RenderUtils.CalcSkip(_pageSize, page);
		var batch = alldata.Skip(RenderUtils.CalcSkip(_pageSize, page)).Take(_pageSize).AsQueryable();
		_dataSource = _dataSource.Concat(batch);
		_page = page;
		if (batch.Count() < _pageSize) _allLoaded = true;
	}

	private async Task _loadMoreAsync()
	{
		_isMoreLoading = true;
		await InvokeAsync(StateHasChanged);
		await loadDataAsync(_page + 1);

		_isMoreLoading = false;
		await InvokeAsync(StateHasChanged);
	}
}