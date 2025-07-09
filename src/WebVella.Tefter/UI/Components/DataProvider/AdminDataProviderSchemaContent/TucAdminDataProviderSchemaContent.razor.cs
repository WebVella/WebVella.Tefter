namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderSchemaContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;

	private TfDataProvider? _provider = null;
	private Guid? _deletedColumnId = null;
	public void Dispose()
	{
		TfSpaceUIService.NavigationDataChanged -= On_NavigationDataChanged;
		TfDataProviderUIService.DataProviderUpdated -= On_DataProviderUpdated;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfSpaceUIService.NavigationDataChanged += On_NavigationDataChanged;
		TfDataProviderUIService.DataProviderUpdated += On_DataProviderUpdated;
	}
	private async void On_NavigationDataChanged(object? caller, TfSpaceNavigationData args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}
	private async void On_DataProviderUpdated(object? caller, TfDataProvider args)
	{
		await _init();
	}

	private async Task _init(TfSpaceNavigationData? navData = null)
	{
		if (navData == null)
			navData = await TfSpaceUIService.GetSpaceNavigationData(Navigator);
		try
		{
			if (navData.State.DataProviderId is null)
			{
				_provider = null;
				await InvokeAsync(StateHasChanged);
				return;
			}
			_provider = TfDataProviderUIService.GetDataProvider(navData.State.DataProviderId.Value);
			if (_provider is null)
				return;
		}
		finally
		{
			UriInitialized = navData.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editColumn(TfDataProviderColumn column)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderColumnManageDialog>(
				column,
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
			ToastService.ShowSuccess(LOC("Column successfully updated!"));
		}
	}

	private async Task _deleteColumn(TfDataProviderColumn column)
	{
		if (_deletedColumnId is not null) return;
		_deletedColumnId = column.Id;

		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?") + "\r\n" + LOC("This will delete all related data too!")))
		{
			_deletedColumnId = null;
			return;
		}
		await InvokeAsync(StateHasChanged);
		try
		{
			TfDataProvider provider = TfDataProviderUIService.DeleteDataProviderColumn(column.Id);
			ToastService.ShowSuccess(LOC("The column is successfully deleted!"));
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

	private async Task _importFromSource()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderImportSchemaDialog>(
				_provider!,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthFullScreen,
					TrapFocus = false,

				});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			//ToastService.ShowSuccess(LOC("Column successfully created!"));
		}
	}
}
