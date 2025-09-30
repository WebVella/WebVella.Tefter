using WebVella.Tefter.Models;

namespace WebVella.Tefter.UI.Components;
public partial class TucHeaderCurrentSpacePages : TfBaseComponent, IDisposable
{
	private List<TfMenuItem> _menu = new();
	private bool _isLoading = true;

	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUIService.SpaceCreated -= On_SpaceChange;
		TfUIService.SpaceUpdated -= On_SpaceChange;
		TfUIService.SpaceDeleted -= On_SpaceChange;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfUIService.SpaceCreated += On_SpaceChange;
		TfUIService.SpaceUpdated += On_SpaceChange;
		TfUIService.SpaceDeleted += On_SpaceChange;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async void On_SpaceChange(object? caller, TfSpace args)
	{
		await _init();
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = await TfUIService.GetNavigationStateAsync(Navigator);

		try
		{
			_menu = (await TfUIService.GetNavigationMenu(Navigator, CurrentUser)).Menu;
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _onClick(TfMenuItem item)
	{
		if (item.Data is null) return;
		if (item.Data.MenuType == TfMenuItemType.CreateSpace)
		{
			await _addSpaceHandler(item);
		}
		else if (item.Data.MenuType == TfMenuItemType.CreateSpacePage)
		{
			await _addSpacePageHandler(item);
		}
		else if (item.Data.MenuType == TfMenuItemType.DeleteSpace)
		{
			await _deleteSpaceHandler(item);
		}
	}

	private async Task _addSpaceHandler(TfMenuItem args)
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceManageDialog>(
		new TfSpace(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TfSpace)result.Data;
			await _init();
		}
	}

	private async Task _deleteSpaceHandler(TfMenuItem args)
	{
		if (args.Data?.SpaceId == null) return;
		try
		{
			TfUIService.DeleteSpace(args.Data.SpaceId.Value);
			ToastService.ShowSuccess(LOC("Space deleted"));
			await _init();
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
	private async Task _addSpacePageHandler(TfMenuItem args)
	{
		if (args.Data?.SpaceId == null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpacePageManageDialog>(
		new TfSpacePage() { SpaceId = args.Data.SpaceId.Value },
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TfSpacePage)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.SpacePagePageUrl, args.Data.SpaceId.Value, item.Id));
		}
	}
}