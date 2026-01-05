namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProviderSchemaContent : TfBaseComponent, IAsyncDisposable
{
	private TfDataProvider? _provider = null;
	private Guid? _deletedColumnId = null;
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
		if (payload is null) return;
		if (payload.DataProvider.Id != _provider?.Id) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
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
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editColumn(TfDataProviderColumn column)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderColumnManageDialog>(
			column,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}

	private async Task _deleteColumn(TfDataProviderColumn column)
	{
		if (_deletedColumnId is not null) return;
		_deletedColumnId = column.Id;

		if (!await JSRuntime.InvokeAsync<bool>("confirm",
			    LOC("Are you sure that you need this column deleted?") + "\r\n" +
			    LOC("This will delete all related data too!")))
		{
			_deletedColumnId = null;
			return;
		}

		await InvokeAsync(StateHasChanged);
		try
		{
			var provider = TfService.DeleteDataProviderColumn(_deletedColumnId.Value);
			ToastService.ShowSuccess(LOC("The column was successfully deleted!"));
			await TfEventBus.PublishAsync(key: TfAuthLayout.GetSessionId().ToString(),
				new TfDataProviderUpdatedEventPayload(provider));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_deletedColumnId = null;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _addColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderColumnManageDialog>(
			new TfDataProviderColumn() { DataProviderId = _provider!.Id },
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}

	private async Task _importFromSource()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderImportSchemaDialog>(
			_provider!,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthFullScreen,
				TrapFocus = false,
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null })
		{
			//ToastService.ShowSuccess(LOC("Column successfully created!"));
		}
	}
}