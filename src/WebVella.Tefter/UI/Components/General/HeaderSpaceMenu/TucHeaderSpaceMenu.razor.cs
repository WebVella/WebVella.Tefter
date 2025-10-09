using WebVella.Tefter.Models;

namespace WebVella.Tefter.UI.Components;
public partial class TucHeaderSpaceMenu : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private List<TfMenuItem> _menu = new();
	private bool _isLoading = true;

	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider?.Dispose();
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.SpacePageCreatedEvent += On_SpacePageChanged;
		TfEventProvider.SpacePageUpdatedEvent += On_SpacePageChanged;
		TfEventProvider.SpacePageDeletedEvent += On_SpacePageChanged;
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_SpacePageChanged(object args)
	{
		await _init(TfAuthLayout.GetState().NavigationState);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_menu = TfAuthLayout.GetState().Menu;
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
			await _init(TfAuthLayout.GetState().NavigationState);
		}
	}

	private async Task _deleteSpaceHandler(TfMenuItem args)
	{
		if (args.Data?.SpaceId == null) return;
		try
		{
			TfService.DeleteSpace(args.Data.SpaceId.Value);
			ToastService.ShowSuccess(LOC("Space deleted"));
			await _init(TfAuthLayout.GetState().NavigationState);
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