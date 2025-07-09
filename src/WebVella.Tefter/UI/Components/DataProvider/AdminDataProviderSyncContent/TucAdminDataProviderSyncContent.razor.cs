namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderSyncContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;

	private TfDataProvider? _provider = null;
	private TfSpaceNavigationData _navData = new();

	private string _nextSyncronization = default!;
	private List<TfDataProviderSynchronizeTask> _syncTasks = new();
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
			_navData = navData;
			_provider = TfDataProviderUIService.GetDataProvider(_navData.State.DataProviderId.Value);
			if (_provider is null)
				return;

			_nextSyncronization = LOC("not scheduled");
			var providerNextTaskCreatedOn = TfDataProviderUIService.GetDataProviderNextSynchronizationTime(_provider.Id);
			if (providerNextTaskCreatedOn is not null)
				_nextSyncronization = providerNextTaskCreatedOn.Value.ToString(TfConstants.DateTimeFormat);

			_syncTasks = TfDataProviderUIService.GetDataProviderSynchronizationTasks(_provider.Id, _navData.State.Page, _navData.State.PageSize ?? TfConstants.PageSize);
		}
		finally
		{
			UriInitialized = navData.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _onViewLogClick(TfDataProviderSynchronizeTask task)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderSyncLogDialog>(
				task,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthExtraLarge,
					TrapFocus = false
				});

	}
	private void _synchronizeNow()
	{
		TfDataProviderUIService.TriggerSynchronization(_provider!.Id);
		ToastService.ShowSuccess(LOC("Synchronization task created!"));
	}
	private async Task _editSchedule()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderSyncManageDialog>(_provider,
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

	private async Task _goFirstPage()
	{
		if (_navData.State.Page == 1) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, 1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goPreviousPage()
	{
		var page = _navData.State.Page - 1;
		if (page < 1) page = 1;
		if (_navData.State.Page == page) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goNextPage()
	{
		if (_syncTasks is null
			|| _syncTasks.Count == 0)
			return;

		var page = _navData.State.Page + 1;
		if (page < 1) page = 1;
		if (_navData.State.Page == page) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goLastPage()
	{
		if (_navData.State.Page == -1) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, -1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goOnPage(int page)
	{
		if (page < 1 && page != -1) page = 1;
		if (_navData.State.Page == page) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}


	private async Task _onColumnsChanged(List<string> columns)
	{
		try
		{
			_provider = TfDataProviderUIService.UpdateDataProviderSynchPrimaryKeyColumns(_provider!.Id, columns);
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
