namespace WebVella.Tefter.UI.Components;
public partial class TucHeaderCurrentSpacePages : TfBaseComponent, IDisposable
{
	[Inject] public ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	private List<TfMenuItem> _menu = new();
	private bool _isLoading = true;

	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfSpaceUIService.SpaceCreated -= On_SpaceChange;
		TfSpaceUIService.SpaceUpdated -= On_SpaceChange;
		TfSpaceUIService.SpaceDeleted -= On_SpaceChange;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfSpaceUIService.SpaceCreated += On_SpaceChange;
		TfSpaceUIService.SpaceUpdated += On_SpaceChange;
		TfSpaceUIService.SpaceDeleted += On_SpaceChange;
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
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);

		var currentUser = await TfUserUIService.GetCurrentUserAsync();
		try
		{
			_menu = (await TfNavigationUIService.GetNavigationMenu(Navigator, currentUser)).Menu;
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
		else if (item.Data.MenuType == TfMenuItemType.CreateSpaceData){ 
			await _addSpaceDataHandler(item);
		}
		else if (item.Data.MenuType == TfMenuItemType.CreateSpaceView){ 
			await _addSpaceViewHandler(item);
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
			Navigator.NavigateTo(string.Format(TfConstants.SpacePageUrl, item.Id));
		}
	}
	private async Task _addSpaceDataHandler(TfMenuItem args)
	{
		if (args.Data?.SpaceId == null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpaceDataManageDialog>(
		new TfSpaceData() { SpaceId = args.Data.SpaceId.Value },
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
			var item = (TfSpaceData)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.SpaceDataPageUrl, args.Data.SpaceId.Value, item.Id));
		}
	}
	private async Task _addSpaceViewHandler(TfMenuItem args)
	{
		if (args.Data?.SpaceId == null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewManageDialog>(
		new TfSpaceView() { SpaceId = args.Data.SpaceId.Value },
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
			var item = (TfSpaceView)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.SpaceViewPageUrl, args.Data.SpaceId.Value, item.Id));
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