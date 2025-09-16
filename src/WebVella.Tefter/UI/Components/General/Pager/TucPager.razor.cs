using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.FluentUI.AspNetCore.Components.Utilities;
using System.Drawing.Printing;

namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Pager.TfPager", "WebVella.Tefter")]
public partial class TucPager : TfBaseComponent, IDisposable
{
	[Inject] private IKeyCodeService KeyCodeService { get; set; } = default!;
	[Inject] private ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	[Parameter] public int? UserPageSize { get; set; } = null;

	[Parameter] public string? Style { get; set; }
	[Parameter] public bool EnableShortcuts { get; set; } = true;

	[Parameter] public EventCallback GoLast { get; set; }
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
	private string _uriInitialized = string.Empty;


	public void Dispose()
	{
		KeyCodeService.UnregisterListener(HandleKeyDownAsync);
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(null);
		KeyCodeService.RegisterListener(HandleKeyDownAsync);
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}
		
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (_uriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		try
		{
			if (!EnableShortcuts)
			{
				_firstTooltip = "Go to first page";
				_prevTooltip = "Go to previous page";
				_nextTooltip = "Go to next page";
				_lastTooltip = "Go to last page";
			}
			else
			{
				_firstTooltip = "Go to first page [Home]";
				_prevTooltip = "Go to previous page [Page Down]";
				_nextTooltip = "Go to next page [Page Up]";
				_lastTooltip = "Go to last page [End]";
			}
			if(navState.Page is null || navState.Page.Value > 0)
				_page = navState.Page ?? 1;

			_pageSize = navState.PageSize ?? (UserPageSize ?? TfConstants.PageSize);
		}
		finally
		{
			_uriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	public Task HandleKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		if (!EnableShortcuts) return Task.CompletedTask;
		if (args.Key == KeyCode.PageUp) _goPrevious();
		else if (args.Key == KeyCode.PageDown) _goNext();
		else if (args.Key == KeyCode.Home) _goFirst();
		else if (args.Key == KeyCode.End) _goLast();

		return Task.CompletedTask;
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
		_page ++;
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await GoOnPage.InvokeAsync(_page);
		}));
	}

	private void _goFirst()
	{
		if(_page == 1) return;
		_page = 1;
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await GoOnPage.InvokeAsync(_page);
		}));
	}

	private void _goPrevious()
	{
		_page--;
		if(_page <= 0)
			_page = 1;
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await GoOnPage.InvokeAsync(_page);
		}));
	}

	private void _pageChanged(int page)
	{
		if(page <= 0) return;
		if (page == _page) return;
		_page = page;
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await GoOnPage.InvokeAsync(page);
		}));
	}

	private void _pageSizeChanged(int pageSize)
	{
		if (pageSize == _pageSize) return;
		_pageSize = pageSize;
		_currentSelectedChangedDebounce.Run(_throttleMS, () => InvokeAsync(async () =>
		{
			await ChangePageSize.InvokeAsync(pageSize);
		}));
	}
}