namespace WebVella.Tefter.UI.Components;

public partial class TucAdminSharedColumnDetailsContent : TfBaseComponent, IAsyncDisposable
{
	private TfSharedColumn? _column = null;
	private bool _isDeleting = false;
	private ReadOnlyCollection<TfDataProvider> _dataProviders = null!;
	private IAsyncDisposable _sharedColumnUpdatedEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _sharedColumnUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_sharedColumnUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSharedColumnUpdatedEventPayload>(
			handler: On_SharedColumnUpdatedEventAsync,
			matchKey: (_) => true);
	}

	private async Task On_SharedColumnUpdatedEventAsync(string? key, TfSharedColumnUpdatedEventPayload? payload)
	{
		if (payload is null) return;
		if(payload.SharedColumn.Id != _column?.Id) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
			await _init(navState: TfAuthLayout.GetState().NavigationState, column: payload.SharedColumn);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState, TfSharedColumn? column = null)
	{
		try
		{
			if (column is not null && column.Id == _column?.Id)
			{
				_column = column;
			}
			else
			{
				var routeData = TfAuthLayout.GetState().NavigationState;
				if (routeData.SharedColumnId is not null)
					_column = TfService.GetSharedColumn(routeData.SharedColumnId.Value);
			}

			if (_column is null) return;
			_dataProviders = TfService.GetSharedColumnConnectedDataProviders(_column.Id);
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editItem()
	{
		if (_column is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSharedColumnManageDialog>(
			_column,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null })
		{
		}
	}

	private async Task _deleteItem()
	{
		if (_column is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm",
			    LOC("Are you sure that you need this column deleted?") + "\r\n" +
			    LOC("This will delete all related data too!")))
			return;
		try
		{
			_isDeleting = true;
			await InvokeAsync(StateHasChanged);
			TfService.DeleteSharedColumn(_column.Id);
			ToastService.ShowSuccess(LOC("The column was successfully deleted!"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSharedColumnDeletedEventPayload(_column));				
			var allColumns = TfService.GetSharedColumns();
			Navigator.NavigateTo(allColumns.Count > 0
				? String.Format(TfConstants.AdminSharedColumnDetailsPageUrl, allColumns[0].Id)
				: TfConstants.AdminSharedColumnsPageUrl);
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

	private string? _getProviderImplementation(TfDataProvider provider)
	{
		if (_column is null) return null;
		if (provider.Identities.Count == 0)
			return null;

		var implementation = provider.Identities.FirstOrDefault(x => x.DataIdentity == _column.DataIdentity);
		if (implementation == null)
			return null;

		return String.Join(", ", implementation.Columns);
	}
}