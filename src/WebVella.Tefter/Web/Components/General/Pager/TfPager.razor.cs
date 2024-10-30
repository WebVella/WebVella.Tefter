namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Pager.TfPager", "WebVella.Tefter")]
public partial class TfPager : TfBaseComponent
{
	[Parameter] public int Page { get; set; }
	[Parameter] public int PageSize { get; set; }

	[Parameter] public EventCallback GoLast { get; set; }
	[Parameter] public EventCallback GoNext { get; set; }
	[Parameter] public EventCallback GoFirst { get; set; }
	[Parameter] public EventCallback GoPrevious { get; set; }
	[Parameter] public EventCallback<int> GoOnPage { get; set; }
	[Parameter] public EventCallback<int> ChangePageSize { get; set; }

	private CancellationTokenSource inputThrottleCancalationToken = new();
	private int _page = 1;
	private int _pageSize = TfConstants.PageSize;
	private int _throttleMS = 500;
	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		_page = Page;
		_pageSize = PageSize;
	}

	private async Task _goLast()
	{
		inputThrottleCancalationToken.Cancel();
		inputThrottleCancalationToken = new();
		await Task.Delay(_throttleMS, inputThrottleCancalationToken.Token).ContinueWith(
		async (task) =>
		{
			await GoLast.InvokeAsync();
		}, inputThrottleCancalationToken.Token);
	}

	private async Task _goNext()
	{
		inputThrottleCancalationToken.Cancel();
		inputThrottleCancalationToken = new();
		await Task.Delay(_throttleMS, inputThrottleCancalationToken.Token).ContinueWith(
		async (task) =>
		{
			await GoNext.InvokeAsync();
		}, inputThrottleCancalationToken.Token);
	}

	private async Task _goFirst()
	{
		inputThrottleCancalationToken.Cancel();
		inputThrottleCancalationToken = new();
		await Task.Delay(_throttleMS, inputThrottleCancalationToken.Token).ContinueWith(
		async (task) =>
		{
			await GoFirst.InvokeAsync();
		}, inputThrottleCancalationToken.Token);
	}

	private async Task _goPrevious()
	{
		inputThrottleCancalationToken.Cancel();
		inputThrottleCancalationToken = new();
		await Task.Delay(_throttleMS, inputThrottleCancalationToken.Token).ContinueWith(
		async (task) =>
		{
			await GoPrevious.InvokeAsync();
		}, inputThrottleCancalationToken.Token);
	}

	private async Task _pageChanged(int page)
	{
		_page = page;
		inputThrottleCancalationToken.Cancel();
		inputThrottleCancalationToken = new();
		await Task.Delay(_throttleMS, inputThrottleCancalationToken.Token).ContinueWith(
		async (task) =>
		{
			await GoOnPage.InvokeAsync(page);
		}, inputThrottleCancalationToken.Token);
	}

	private async Task _pageSizeChanged(int pageSize)
	{
		_pageSize = pageSize;
		inputThrottleCancalationToken.Cancel();
		inputThrottleCancalationToken = new();
		await Task.Delay(_throttleMS, inputThrottleCancalationToken.Token).ContinueWith(
		async (task) =>
		{
			await ChangePageSize.InvokeAsync(pageSize);
		}, inputThrottleCancalationToken.Token);
	}
}