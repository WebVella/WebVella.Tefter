

namespace WebVella.Tefter.Demo.Components;
public partial class WvBookmarks : WvBaseComponent
{
	private List<SpaceView> _bookmarks = new();
	private bool _isLoading = true;
	private string _search = null;
	private int _page = 1;
	private int _pageSize = WvConstants.CardPageSize;
	private bool _showMoreVisible = true;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			WvState.FilterChanged -= searchHandler;
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			loadData(null, 1);
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
			WvState.FilterChanged += searchHandler;
		}
	}

	private void loadData(string search, int page)
	{
		if (page == 1) _bookmarks.Clear();

		var batch = WvService.GetBookmaredByUserId(search, WvState.GetUser().Id, page, _pageSize);
		_bookmarks.AddRange(batch);
		_page = page;
		_search = search;
		_showMoreVisible = batch.Count == _pageSize;
	}

	private void showMoreClick()
	{
		loadData(null, _page + 1);
	}

	private void searchHandler(object sender, StateFilterChangedEventArgs args)
	{
		base.InvokeAsync(async () =>
		{
			if (_search == args.Filter.Search) return;
			loadData(args.Filter.Search, 1);
			await InvokeAsync(StateHasChanged);
		});
	}

	private void cardClicked(SpaceView view)
	{
		Navigator.NavigateTo($"/space/{view.SpaceId}/data/{view.SpaceDataId}/view/{view.Id}");
	}
}