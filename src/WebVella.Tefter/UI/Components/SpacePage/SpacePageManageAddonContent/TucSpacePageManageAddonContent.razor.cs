namespace WebVella.Tefter.UI.Components;
public partial class TucSpacePageManageAddonContent : TfBaseComponent, IDisposable
{
	private bool _isDeleting = false;
	private TfSpace _space = default!;
	private TfSpacePage _spacePage = default!;
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUIService.SpacePageUpdated -= On_SpacePageUpdated;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfUIService.SpacePageUpdated += On_SpacePageUpdated;
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async void On_SpacePageUpdated(object? caller, TfSpacePage args)
	{
		await _init(null);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState == null)
			navState = TfAuthLayout.NavigationState;
		try
		{
			if (navState.SpaceId is null)
				throw new Exception("Space Id not found in URL");
			_space = TfUIService.GetSpace(navState.SpaceId.Value);
			if (navState.SpacePageId is null)
				throw new Exception("Page Id not found in URL");
			_spacePage = TfUIService.GetSpacePage(navState.SpacePageId.Value);
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editProvider()
	{
		// if (_provider is null) return;
		// var dialog = await DialogService.ShowDialogAsync<TucDataProviderManageDialog>(
		// _provider,
		// new DialogParameters()
		// {
		// 	PreventDismissOnOverlayClick = true,
		// 	PreventScroll = true,
		// 	Width = TfConstants.DialogWidthLarge,
		// 	TrapFocus = false
		// });
		// var result = await dialog.Result;
		// if (!result.Cancelled && result.Data != null)
		// {
		// }
	}

	private async Task _deleteProvider()
	{
		// if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this data provider deleted?") + "\r\n" + LOC("Will proceeed only if there are not existing columns attached")))
		// 	return;
		//
		// if (_provider is null) return;
		// try
		// {
		// 	_isDeleting = true;
		// 	await InvokeAsync(StateHasChanged);
		// 	TfUIService.DeleteDataProvider(_provider.Id);
		// 	var providers = TfUIService.GetDataProviders();
		// 	_provider = null;
		// 	ToastService.ShowSuccess(LOC("Data provider was successfully deleted"));
		// 	if (providers.Count > 0)
		// 	{
		// 		Navigator.NavigateTo(String.Format(TfConstants.AdminDataProviderDetailsPageUrl, providers[0].Id));
		// 	}
		// 	else
		// 	{
		// 		Navigator.NavigateTo(TfConstants.AdminDataProvidersPageUrl, true);
		// 	}
		// }
		// catch (Exception ex)
		// {
		// 	ProcessException(ex);
		// }
		// finally
		// {
		// 	_isDeleting = false;
		// 	await InvokeAsync(StateHasChanged);
		// }

	}
}
