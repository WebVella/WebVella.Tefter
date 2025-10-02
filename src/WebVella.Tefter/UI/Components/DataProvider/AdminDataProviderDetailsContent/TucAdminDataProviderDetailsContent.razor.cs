namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderDetailsContent : TfBaseComponent, IDisposable
{
	private TfDataProvider? _provider = null;
	private TfDataProviderDisplaySettingsScreenRegionContext? _dynamicComponentContext = null;
	private TfScreenRegionScope? _dynamicComponentScope = null;
	private bool _isDeleting = false;
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUIService.DataProviderUpdated -= On_DataProviderUpdated;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.NavigationState);
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfUIService.DataProviderUpdated += On_DataProviderUpdated;
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async void On_DataProviderUpdated(object? caller, TfDataProvider args)
	{
		await _init(TfAuthLayout.NavigationState);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			if (navState.DataProviderId is null)
			{
				_provider = null;
				await InvokeAsync(StateHasChanged);
				return;
			}
			_provider = TfUIService.GetDataProvider(navState.DataProviderId.Value);
			if (_provider is null)
				return;
			_dynamicComponentContext = new TfDataProviderDisplaySettingsScreenRegionContext()
			{
				SettingsJson = _provider.SettingsJson
			};
			_dynamicComponentScope = new TfScreenRegionScope(_provider.ProviderType.GetType(), null);
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editProvider()
	{
		if (_provider is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderManageDialog>(
		_provider,
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
		}
	}

	private async Task _deleteProvider()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this data provider deleted?") + "\r\n" + LOC("Will proceeed only if there are not existing columns attached")))
			return;

		if (_provider is null) return;
		try
		{
			_isDeleting = true;
			await InvokeAsync(StateHasChanged);
			TfUIService.DeleteDataProvider(_provider.Id);
			var providers = TfUIService.GetDataProviders();
			_provider = null;
			ToastService.ShowSuccess(LOC("Data provider was successfully deleted"));
			if (providers.Count > 0)
			{
				Navigator.NavigateTo(String.Format(TfConstants.AdminDataProviderDetailsPageUrl, providers[0].Id));
			}
			else
			{
				Navigator.NavigateTo(TfConstants.AdminDataProvidersPageUrl, true);
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isDeleting = false;
			await InvokeAsync(StateHasChanged);
		}

	}
}
