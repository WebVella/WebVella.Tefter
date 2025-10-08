namespace WebVella.Tefter.UI.Components;
public partial class TucAdminSharedColumnDetailsContent : TfBaseComponent, IDisposable
{
	private TfSharedColumn? _column = null;
	private bool _isDeleting = false;

	internal ReadOnlyCollection<TfDataProvider> _dataProviders = null!;
	public void Dispose()
	{
		TfEventProvider.SharedColumnUpdatedEvent -= On_SharedColumnUpdated;
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfState.NavigationState);
		TfEventProvider.SharedColumnUpdatedEvent += On_SharedColumnUpdated;
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async void On_SharedColumnUpdated(TfSharedColumnUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(navState: TfState.NavigationState, column: args.Payload);
		});
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(navState: args);
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
				var routeData = TfState.NavigationState;
				if (routeData.SharedColumnId is not null)
					_column = TfService.GetSharedColumn(routeData.SharedColumnId.Value);
			}
			if(_column is null) return;
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
		var dialog = await DialogService.ShowDialogAsync<TucSharedColumnManageDialog>(
				_column,
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

	private async Task _deleteItem()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?") + "\r\n" + LOC("This will delete all related data too!")))
			return;
		try
		{
			_isDeleting = true;
			await InvokeAsync(StateHasChanged);
			TfService.DeleteSharedColumn(_column.Id);
			ToastService.ShowSuccess(LOC("The column is successfully deleted!"));
			var allColumns = TfService.GetSharedColumns();
			if (allColumns.Count > 0)
				Navigator.NavigateTo(String.Format(TfConstants.AdminSharedColumnDetailsPageUrl, allColumns[0].Id));
			else
				Navigator.NavigateTo(TfConstants.AdminSharedColumnsPageUrl);
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
		if (provider.Identities is null || provider.Identities.Count == 0)
			return null;

		var implementation = provider.Identities.FirstOrDefault(x => x.DataIdentity == _column.DataIdentity);
		if (implementation == null)
			return null;

		return String.Join(", ", implementation.Columns);
	}
}