namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderSynchronization.TfAdminDataProviderSynchronization", "WebVella.Tefter")]
public partial class TfAdminDataProviderSynchronization : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private List<string> _keyitems = new();
	private string _nextSyncronization;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (TfAppState.Value.AdminDataProvider is null)
			throw new Exception("Data provider not initialized");
		if (TfAppState.Value.AdminDataProvider?.SynchPrimaryKeyColumns is not null)
			_keyitems = TfAppState.Value.AdminDataProvider.SynchPrimaryKeyColumns.ToList();
		_setNextSyncOn();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
		}
	}

	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(async () =>
		{
			_setNextSyncOn();
			await InvokeAsync(StateHasChanged);
		});
	}

	private void _setNextSyncOn(){ 
		if(TfAppState.Value.AdminDataProvider is null) return;
		_nextSyncronization = LOC("not scheduled");
		var providerNextTaskCreatedOn = UC.GetProviderNextSyncOn(TfAppState.Value.AdminDataProvider.Id);
		if (providerNextTaskCreatedOn is not null)
			_nextSyncronization = providerNextTaskCreatedOn.Value.ToString(TfConstants.DateTimeFormat);	
	}

	private async Task _onViewLogClick(TucDataProviderSyncTask task)
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderSyncLogDialog>(
				task,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthExtraLarge,
					TrapFocus = false
				});

	}

	private async Task _synchronizeNow()
	{
		await UC.TriggerSynchronization(TfAppState.Value.AdminDataProvider.Id);
		ToastService.ShowSuccess(LOC("Synchronization task created!"));
		Navigator.ReloadCurrentUrl();
	}

	private async Task _editSchedule()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderSyncManageDialog>(TfAppState.Value.AdminDataProvider,
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
			var record = (TucDataProvider)result.Data;
			ToastService.ShowSuccess(LOC("Provider successfully updated!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = record }));
		}
	}

	private async Task _goFirstPage()
	{
		if (TfAppState.Value.Route.Page == 1) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, 1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goPreviousPage()
	{
		var page = TfAppState.Value.Route.Page - 1;
		if (page < 1) page = 1;
		if (TfAppState.Value.Route.Page == page) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goNextPage()
	{
		if (TfAppState.Value.DataProviderSyncTasks is null
			|| TfAppState.Value.DataProviderSyncTasks.Count == 0)
			return;

		var page = TfAppState.Value.Route.Page + 1;
		if (page < 1) page = 1;
		if (TfAppState.Value.Route.Page == page) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goLastPage()
	{
		if (TfAppState.Value.Route.Page == -1) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, -1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goOnPage(int page)
	{
		if (page < 1 && page != -1) page = 1;
		if (TfAppState.Value.Route.Page == page) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}


	private async Task _onColumnsChanged(List<string> columns)
	{
		try
		{
			TucDataProvider provider = UC.UpdateDataProviderSynchPrimaryKeyColumns(TfAppState.Value.AdminDataProvider.Id, columns);
			ToastService.ShowSuccess("Data provider updated!");
			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					AdminDataProvider = provider,

				}));
			_keyitems = provider.SynchPrimaryKeyColumns.ToList();
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