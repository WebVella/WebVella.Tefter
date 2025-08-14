namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderAuxContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] public ITfDataIdentityUIService TfDataIdentityUIService { get; set; } = default!;

	private TfDataProvider? _provider = null;
	public bool _allIdentitiesImplemented = false;
	private List<TfDataProvider> _connectedProviders = new();
	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfDataProviderUIService.DataProviderUpdated -= On_DataProviderUpdated;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();

		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfDataProviderUIService.DataProviderUpdated += On_DataProviderUpdated;
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async void On_DataProviderUpdated(object? caller, TfDataProvider args)
	{
		await _init();
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState == null)
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);

		try
		{
			if (navState.DataProviderId is null)
			{
				_provider = null;
				await InvokeAsync(StateHasChanged);
				return;
			}
			_provider = TfDataProviderUIService.GetDataProvider(navState.DataProviderId.Value);
			if (_provider is null)
				return;

			var allIdentities = TfDataIdentityUIService.GetDataIdentities();
			_allIdentitiesImplemented = true;
			foreach (var item in allIdentities)
			{
				if (!_provider.Identities.Any(x => x.DataIdentity == item.DataIdentity))
					_allIdentitiesImplemented = false;
			}
			_connectedProviders = TfDataProviderUIService.GetDataProviderConnectedProviders(_provider.Id);
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _implementDataIdentity()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderIdentityManageDialog>(
		new TfDataProviderIdentity() { DataProviderId = _provider!.Id },
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

	private async Task _editIdentity(TfDataProviderIdentity identity)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderIdentityManageDialog>(
				identity,
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

	private async Task _deleteIdentity(TfDataProviderIdentity key)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this key deleted?")))
			return;
		try
		{
			var provider = TfDataProviderUIService.DeleteDataProviderIdentity(key.Id);
			ToastService.ShowSuccess(LOC("The implementation is successfully deleted!"));

		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private string _showCommonIdentities(TfDataProvider subProvider)
	{
		var commonIdentities = _provider.Identities.Where(x => x.DataIdentity != TfConstants.TF_ROW_ID_DATA_IDENTITY)
							.Select(x => x.DataIdentity)
							.Intersect(subProvider.Identities.Where(x => x.DataIdentity != TfConstants.TF_ROW_ID_DATA_IDENTITY).Select(x => x.DataIdentity)).ToList();
		if (commonIdentities.Count == 0) return "n/a";
		return String.Join(", ", commonIdentities);
	}
}
