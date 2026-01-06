namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProviderDatasetsContent : TfBaseComponent, IAsyncDisposable
{
	TfDataProvider? _provider = null;
	List<TfDataset> _items = new();
	private IAsyncDisposable _datasetEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _datasetEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);

		Navigator.LocationChanged += On_NavigationStateChanged;
		_datasetEventSubscriber = await TfEventBus.SubscribeAsync<TfDatasetEventPayload>(
			handler: On_DatasetEventAsync,
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

	private async Task On_DatasetEventAsync(string? key, TfDatasetEventPayload? payload)
	{
		if(payload is null) return;
		if(payload.Dataset.DataProviderId != _provider?.Id) return;
		if(key == TfAuthLayout.GetSessionId().ToString())
			await _init(TfAuthLayout.GetState().NavigationState);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());		
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
			_items = TfService.GetDatasets(providerId: _provider.Id);
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _createDataset()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDatasetManageDialog>(
			new TfDataset() { DataProviderId = _provider!.Id },
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

	private async Task _editDataset(TfDataset dataset)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDatasetManageDialog>(
			dataset,
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

	private async Task _deleteDataset(TfDataset dataset)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this key deleted?")))
			return;
		try
		{
			TfService.DeleteDataset(dataset.Id);
			ToastService.ShowSuccess(LOC("The dataset was successfully deleted!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfDatasetDeletedEventPayload(dataset));				
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private async Task _copyDataset(TfDataset dataset)
	{
		try
		{
			TfService.CopyDataset(dataset.Id);
			ToastService.ShowSuccess(LOC("The dataset was successfully copied!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfDatasetDeletedEventPayload(dataset));				
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}	
	
	private async Task _manageColumns(TfDataset dataset)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDatasetColumnsDialog>(
			dataset,
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

	private async Task _manageFilters(TfDataset dataset)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDatasetFiltersDialog>(
			dataset,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthExtraLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null }) { }
	}

	private async Task _manageSort(TfDataset dataset)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDatasetSortOrderDialog>(
			dataset,
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

	private async Task _viewData(TfDataset dataset)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDatasetDataDialog>(
			dataset,
			new()
			{
				PreventDismissOnOverlayClick = false,
				PreventScroll = true,
				Width = TfConstants.DialogWidthFullScreen,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null }) { }
	}
}