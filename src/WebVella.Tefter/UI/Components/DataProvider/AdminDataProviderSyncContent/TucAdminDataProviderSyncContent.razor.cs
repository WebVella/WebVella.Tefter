namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProviderSyncContent : TfBaseComponent, IAsyncDisposable
{
	private TfDataProvider? _provider = null;
	private TfNavigationState _navState = new();

	private string _nextSyncronization = null!;
	private List<TfDataProviderSynchronizeTask> _syncTasks = new();
	private IAsyncDisposable _dataProviderUpdatedEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _dataProviderUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_dataProviderUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfDataProviderUpdatedEventPayload>(
			handler: On_DataProviderUpdatedEventAsync,
			matchKey: (_) => true);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_DataProviderUpdatedEventAsync(string? key, TfDataProviderUpdatedEventPayload? payload)
	{
		if(payload is null) return;
		if(payload.DataProvider.Id != _provider?.Id) return;
		if(key == TfAuthLayout.GetSessionId().ToString())
			await _init(TfAuthLayout.GetState().NavigationState);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}		

	private async Task _init(TfNavigationState navState)
	{
		if (navState.DataProviderId is null)
		{
			_provider = null;
			await InvokeAsync(StateHasChanged);
			return;
		}

		_navState = navState;
		try
		{
			_provider = TfService.GetDataProvider(_navState.DataProviderId.Value);
			if (_provider is null)
				return;

			_nextSyncronization = LOC("not scheduled");
			var providerNextTaskCreatedOn = TfService.GetDataProviderNextSynchronizationTime(_provider.Id);
			if (providerNextTaskCreatedOn is not null)
				_nextSyncronization = providerNextTaskCreatedOn.Value.ToString(TfConstants.DateTimeFormat);

			_syncTasks = TfService.GetDataProviderSynchronizationTasks(_provider.Id, _navState.Page,
				_navState.PageSize ?? TfConstants.PageSize);
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _onViewLogClick(TfDataProviderSynchronizeTask task)
	{
		_ = await DialogService.ShowDialogAsync<TucDataProviderSyncLogDialog>(
			task,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthExtraLarge,
				TrapFocus = false
			});
	}

	private void _synchronizeNow()
	{
		if (_provider is null) return;
		if (_provider.Columns.Count == 0)
		{
			ToastService.ShowWarning(LOC("No provider columns created yet!"));
			return;
		}

		TfService.TriggerSynchronization(_provider!.Id);
		ToastService.ShowSuccess(LOC("Synchronization task created!"));
	}

	private async Task _editSchedule()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderSyncManageDialog>(_provider!,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null }) { }
	}

	private async Task _goLastPage()
	{
		if (_navState.Page == -1) return;
		var queryDict = new Dictionary<string, object?> { { TfConstants.PageQueryName, -1 } };
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _goOnPage(int page)
	{
		if (page < 1 && page != -1) page = 1;
		if (_navState.Page == page) return;
		var queryDict = new Dictionary<string, object?> { { TfConstants.PageQueryName, page } };
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}


	private async Task _onColumnsChanged(List<string> columns)
	{
		try
		{
			TfService.UpdateDataProviderSynchPrimaryKeyColumns(_provider!.Id, columns);
			_provider = TfService.GetDataProvider(_provider!.Id);
			ToastService.ShowSuccess("Data provider updated!");
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}
}