﻿namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProviderSyncContent : TfBaseComponent, IDisposable
{
	private TfDataProvider? _provider = null;
	private TfNavigationState _navState = new();

	private string _nextSyncronization = null!;
	private List<TfDataProviderSynchronizeTask> _syncTasks = new();

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
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(args);
		});
	}

	private async void On_DataProviderUpdated(TfDataProviderUpdatedEvent args)
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
			_navState = navState;
			_provider = TfService.GetDataProvider(_navState.DataProviderId.Value);
			if (_provider is null)
				return;

			_nextSyncronization = LOC("not scheduled");
			var providerNextTaskCreatedOn = TfService.GetDataProviderNextSynchronizationTime(_provider.Id);
			if (providerNextTaskCreatedOn is not null)
				_nextSyncronization = providerNextTaskCreatedOn.Value.ToString(TfConstants.DateTimeFormat);

			_syncTasks = TfService.GetDataProviderSynchronizationTasks(_provider.Id, _navState.Page, _navState.PageSize ?? TfConstants.PageSize);
		}
		finally
		{
			UriInitialized = _navState.Uri;
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

	private async Task _goLastPage()
	{
		if (_navState.Page == -1) return;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.PageQueryName, -1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goOnPage(int page)
	{
		if (page < 1 && page != -1) page = 1;
		if (_navState.Page == page) return;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.PageQueryName, page}
		};
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
