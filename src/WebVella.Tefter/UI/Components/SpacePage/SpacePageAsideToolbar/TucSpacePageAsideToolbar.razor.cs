using WebVella.Tefter.Models;

namespace WebVella.Tefter.UI.Components;

public partial class TucSpacePageAsideToolbar : TfBaseComponent, IDisposable
{
	private string? _search = null;
	private TfNavigationState _navState = new();
	private TfSpaceNavigationActiveTab _activeTab = TfSpaceNavigationActiveTab.Pages;
	private bool _actionMenuOpened = false;
	public void Dispose()
	{
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfState.NavigationState);
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async Task On_NavigationStateChanged(TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(args);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;

		try
		{
			_actionMenuOpened = false;
			_search = _navState.SearchAside;
			_activeTab = NavigatorExt.GetEnumFromQuery<TfSpaceNavigationActiveTab>(Navigator, TfConstants.TabQueryName, TfSpaceNavigationActiveTab.Pages)!.Value;
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}


	private async Task onSearch(string search)
	{
		_search = search;
		await NavigatorExt.ApplyChangeToUrlQuery(Navigator, TfConstants.AsideSearchQueryName, search);
	}

	private async Task _setActiveTab(TfSpaceNavigationActiveTab tab)
	{
		if (_activeTab == tab) return;
		_activeTab = tab;
		await NavigatorExt.ApplyChangeToUrlQuery(Navigator, TfConstants.TabQueryName, _activeTab);
	}

	private async Task _addPageAsync()
	{
		if(_navState.SpaceId is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
		new TfSpacePage() { SpaceId = _navState.SpaceId.Value },
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null){
			var newPage = (TfSpacePage)result.Data;
			Navigator.NavigateTo(String.Format(TfConstants.SpacePagePageUrl, newPage.SpaceId, newPage.Id));
		}
	}

	private async Task _editSpaceAsync()
	{
		if (_navState.SpaceId is null) return;
		var space = TfService.GetSpace(_navState.SpaceId.Value);
		var dialog = await DialogService.ShowDialogAsync<TucSpaceManageDialog>(
		space,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}

	private async Task _deleteSpaceAsync()
	{
		if (_navState.SpaceId is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space deleted?")))
			return;
		try
		{
			TfService.DeleteSpace(_navState.SpaceId.Value);
			ToastService.ShowSuccess(LOC("Space deleted"));
			Navigator.NavigateTo(TfConstants.HomePageUrl);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private async Task _editSpaceAccessAsync()
	{
		if (_navState.SpaceId is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpaceAccessDialog>(
		_navState.SpaceId,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}

	private async Task _managePagesAsync() {
		if (_navState.SpaceId is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpacePagesDialog>(
		_navState.SpaceId,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
		
	}

}