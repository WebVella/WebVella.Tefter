namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProviderDatasetsContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] public ITfDatasetUIService TfDatasetUIService { get; set; } = default!;

	TfDataProvider? _provider = null;
	List<TfDataset> _items = new();

	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfDatasetUIService.DatasetCreated -= On_DatasetChanged;
		TfDatasetUIService.DatasetUpdated -= On_DatasetChanged;
		TfDatasetUIService.DatasetDeleted -= On_DatasetChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();

		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfDatasetUIService.DatasetCreated += On_DatasetChanged;
		TfDatasetUIService.DatasetUpdated += On_DatasetChanged;
		TfDatasetUIService.DatasetDeleted += On_DatasetChanged;
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async void On_DatasetChanged(object? caller, TfDataset args)
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
			_items = TfDatasetUIService.GetDatasets(providerId:_provider.Id);
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

	private async Task _editDataset(TfDataset dataset)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDatasetManageDialog>(
				dataset,
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

	private async Task _deleteDataset(TfDataset dataset)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this key deleted?")))
			return;
		try
		{
			TfDatasetUIService.DeleteDataset(dataset.Id);
			ToastService.ShowSuccess(LOC("The implementation is successfully deleted!"));

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

	private async Task _manageFilters(TfDataset dataset)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDatasetFiltersDialog>(
				dataset,
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

	private async Task _manageSort(TfDataset dataset)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDatasetSortOrderDialog>(
				dataset,
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

	private async Task _viewData(TfDataset dataset)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDatasetDataDialog>(
				dataset,
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

}
