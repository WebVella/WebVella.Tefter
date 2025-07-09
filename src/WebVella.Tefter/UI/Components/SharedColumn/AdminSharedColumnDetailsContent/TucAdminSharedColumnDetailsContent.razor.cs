namespace WebVella.Tefter.UI.Components;
public partial class TucAdminSharedColumnDetailsContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfSharedColumnUIService TfSharedColumnUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;

	private TfSharedColumn? _column = null;
	private bool _isDeleting = false;

	internal ReadOnlyCollection<TfDataProvider> _dataProviders = default!;
	public void Dispose()
	{
		TfSharedColumnUIService.SharedColumnUpdated -= On_SharedColumnUpdated;
		TfSpaceUIService.NavigationDataChanged -= On_NavigationDataChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfSharedColumnUIService.SharedColumnUpdated += On_SharedColumnUpdated;
		TfSpaceUIService.NavigationDataChanged += On_NavigationDataChanged;
	}

	private async void On_SharedColumnUpdated(object? caller, TfSharedColumn column)
	{
		await _init(column: column);
	}

	private async void On_NavigationDataChanged(object? caller, TfSpaceNavigationData args)
	{
		if (UriInitialized != args.Uri)
			await _init(navData: args);
	}

	private async Task _init(TfSpaceNavigationData? navData = null, TfSharedColumn? column = null)
	{
		if (navData == null)
			navData = await TfSpaceUIService.GetSpaceNavigationData(Navigator);

		try
		{
			if (column is not null && column.Id == _column?.Id)
			{
				_column = column;
			}
			else
			{
				var routeData = Navigator.GetRouteState();
				if (routeData.SharedColumnId is not null)
					_column = TfSharedColumnUIService.GetSharedColumn(routeData.SharedColumnId.Value);
			}
			if(_column is null) return;
			_dataProviders = TfSharedColumnUIService.GetSharedColumnConnectedDataProviders(_column.Id);
		}
		finally
		{
			UriInitialized = navData.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editRole()
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

	private async Task _deleteRole()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?") + "\r\n" + LOC("This will delete all related data too!")))
			return;
		try
		{
			_isDeleting = true;
			await InvokeAsync(StateHasChanged);
			TfSharedColumnUIService.DeleteSharedColumn(_column.Id);
			ToastService.ShowSuccess(LOC("The column is successfully deleted!"));
			var allColumns = TfSharedColumnUIService.GetSharedColumns();
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