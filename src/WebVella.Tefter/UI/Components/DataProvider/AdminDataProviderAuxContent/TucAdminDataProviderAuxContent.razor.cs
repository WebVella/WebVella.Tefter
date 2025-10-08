namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderAuxContent : TfBaseComponent, IDisposable
{
	private TfDataProvider? _provider = null;
	private List<TfDataProvider> _connectedProviders = new();
	public void Dispose()
	{
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
		TfEventProvider.DataProviderUpdatedEvent -= On_DataProviderUpdated;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfState.NavigationState);

		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
		TfEventProvider.DataProviderUpdatedEvent += On_DataProviderUpdated;
	}
	private async Task On_NavigationStateChanged(TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(args);
		});
	}

	private async Task On_DataProviderUpdated(TfDataProviderUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfState.NavigationState);
		});
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
			_provider = TfService.GetDataProvider(navState.DataProviderId.Value);
			if (_provider is null)
				return;
			_connectedProviders = TfService.GetDataProviderConnectedProviders(_provider.Id);
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
			var provider = TfService.DeleteDataProviderIdentity(key.Id);
			ToastService.ShowSuccess(LOC("The implementation is successfully deleted!"));

		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private string _showCommonIdentities(TfDataProvider subProvider)
	{
		var commonIdentities = _provider.Identities
							.Select(x => x.DataIdentity)
							.Intersect(subProvider.Identities.Select(x => x.DataIdentity)).ToList();
		if (commonIdentities.Count == 0) return "n/a";
		return String.Join(", ", commonIdentities);
	}
}
