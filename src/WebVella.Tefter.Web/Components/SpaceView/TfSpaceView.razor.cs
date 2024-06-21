namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceView : TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private IQueryable<DataRow> _data = Enumerable.Empty<DataRow>().AsQueryable();
	private bool _isGridLoading = true;
	private bool _isMoreLoading = false;
	private Guid? _loadedSpaceViewId = null;
	private bool _allLoaded = false;
	private int _page = 1;
	private int _pageSize = 30;// TfConstants.PageSize;
	private HashSet<Guid> _renderedHs = new HashSet<Guid>();

	//private RenderFragment options = builder =>
	//{
	//	builder.OpenElement(0, "div");
	//	builder.AddContent(1, "Hello from RenderFragment!");
	//	builder.CloseElement();
	//};



	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			SessionState.StateChanged -= SessionState_StateChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (SessionState.Value.SpaceView is not null)
			{
				await loadDataAsync(1);
				SessionState.StateChanged += SessionState_StateChanged;
			}
			_isGridLoading = false;
			await InvokeAsync(StateHasChanged);

		}
	}

	protected void SessionState_StateChanged(object sender, EventArgs e)
	{
		InvokeAsync(async () =>
		{
			if (SessionState.Value.SpaceView?.Id == _loadedSpaceViewId) return;

			_isGridLoading = true;
			await InvokeAsync(StateHasChanged);
			await loadDataAsync(1);
			_isGridLoading = false;
			await InvokeAsync(StateHasChanged);
		});

	}
	private async Task loadDataAsync(int page)
	{
		if (SessionState.Value.SpaceView is null) return;
		var alldata = await TfSrv.GetSpaceViewData(SessionState.Value.SpaceView.Id);
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
		_loadedSpaceViewId = SessionState.Value.SpaceView.Id;
	}

	private void addActions(DataRow row)
	{
		row.OnSelect = () => { onRowSelect(row); };
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