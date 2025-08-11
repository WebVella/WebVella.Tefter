using Microsoft.FluentUI.AspNetCore.Components.Utilities;

namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Pager.TfPager", "WebVella.Tefter")]
public partial class TucPager : TfBaseComponent, IDisposable
{
	[Inject] private IKeyCodeService KeyCodeService { get; set; } = default!;
	[Parameter] public int Page { get; set; }
	[Parameter] public int PageSize { get; set; }
	[Parameter] public string? Style { get; set; }
	[Parameter] public bool EnableShortcuts { get; set; } = true;

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

	private string _firstTooltip = "Go to first page [Home]";
	private string _prevTooltip = "Go to previous page [Page Down]";
	private string _nextTooltip = "Go to next page [Page Up]";
	private string _lastTooltip = "Go to last page [End]";

	public void Dispose()
	{
		KeyCodeService.UnregisterListener(HandleKeyDownAsync);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			KeyCodeService.RegisterListener(HandleKeyDownAsync);
		}
	}

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		_page = Page;
		_pageSize = PageSize;

		if (!EnableShortcuts)
		{
			_firstTooltip = "Go to first page";
			_prevTooltip = "Go to previous page";
			_nextTooltip = "Go to next page";
			_lastTooltip = "Go to last page";
		}
		else{ 
			_firstTooltip = "Go to first page [Home]";
			_prevTooltip = "Go to previous page [Page Down]";
			_nextTooltip = "Go to next page [Page Up]";
			_lastTooltip = "Go to last page [End]";		
		}
	}

	public async Task HandleKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		if (!EnableShortcuts) return;
		if (args.Key == KeyCode.PageUp) await GoPrevious.InvokeAsync();
		else if (args.Key == KeyCode.PageDown) await GoNext.InvokeAsync();
		else if (args.Key == KeyCode.Home) await GoFirst.InvokeAsync();
		else if (args.Key == KeyCode.End) await GoLast.InvokeAsync(); ;
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
		if (page == _page) return;
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await GoOnPage.InvokeAsync(page);
		}));
	}

	private void _pageSizeChanged(int pageSize)
	{
		if (pageSize == _pageSize) return;
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await ChangePageSize.InvokeAsync(pageSize);
		}));
	}
}