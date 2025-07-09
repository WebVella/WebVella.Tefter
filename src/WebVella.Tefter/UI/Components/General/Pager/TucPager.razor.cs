using Microsoft.FluentUI.AspNetCore.Components.Utilities;

namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Pager.TfPager", "WebVella.Tefter")]
public partial class TucPager : TfBaseComponent
{
	[Parameter] public int Page { get; set; }
	[Parameter] public int PageSize { get; set; }
	[Parameter] public string? Style { get; set; }

	[Parameter] public EventCallback GoLast { get; set; }
	[Parameter] public EventCallback GoNext { get; set; }
	[Parameter] public EventCallback GoFirst { get; set; }
	[Parameter] public EventCallback GoPrevious { get; set; }
	[Parameter] public EventCallback<int> GoOnPage { get; set; }
	[Parameter] public EventCallback<int> ChangePageSize { get; set; }

	private readonly Debounce _currentSelectedChangedDebounce = new();
	private int _page = 1;
	private int _pageSize = TfConstants.PageSize;
	private int _throttleMS = 500;

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		_page = Page;
		_pageSize = PageSize;
	}

	private void _goLast()
	{
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await GoLast.InvokeAsync();
		}));
	}

	private void _goNext()
	{
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await GoNext.InvokeAsync();
		}));
	}

	private void _goFirst()
	{
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await GoFirst.InvokeAsync();
		}));
	}

	private void _goPrevious()
	{
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await GoPrevious.InvokeAsync();
		}));
	}

	private void _pageChanged(int page)
	{
		_page = page;
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await GoOnPage.InvokeAsync(page);
		}));
	}

	private void _pageSizeChanged(int pageSize)
	{
		_pageSize = pageSize;
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await ChangePageSize.InvokeAsync(pageSize);
		}));
	}
}