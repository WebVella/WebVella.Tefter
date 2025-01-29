namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderSynchronization.TfAdminDataProviderSynchronization", "WebVella.Tefter")]
public partial class TfAdminDataProviderSynchronization : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private List<string> _keyitems = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (TfAppState.Value.AdminDataProvider?.SynchPrimaryKeyColumns is not null)
			_keyitems = TfAppState.Value.AdminDataProvider.SynchPrimaryKeyColumns.ToList();
	}

	private async Task _onViewLogClick(Guid taskId, TucDataProviderSyncTaskInfoType type)
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderSyncLogDialog>(
				new TucDataProviderSyncTaskInfoLog()
				{
					Type = type,
					TaskId = taskId
				},
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

	private async Task _goFirstPage()
	{
		if (TfAppState.Value.Route.Page == 1) return;
		var queryDict = new Dictionary<string, object>{
			{TfConstants.PageQueryName, 1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goPreviousPage()
	{
		var page = TfAppState.Value.Route.Page - 1;
		if (page < 1) page = 1;
		if (TfAppState.Value.Route.Page == page) return;
		var queryDict = new Dictionary<string, object>{
			{TfConstants.PageQueryName, page}
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
			{TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goLastPage()
	{
		if (TfAppState.Value.Route.Page == -1) return;
		var queryDict = new Dictionary<string, object>{
			{TfConstants.PageQueryName, -1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goOnPage(int page)
	{
		if (page < 1 && page != -1) page = 1;
		if (TfAppState.Value.Route.Page == page) return;
		var queryDict = new Dictionary<string, object>{
			{TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}


	private async Task _onColumnsChanged(List<string> columns)
	{
		try
		{
			Result<TucDataProvider> submitResult = UC.UpdateDataProviderSynchPrimaryKeyColumns(TfAppState.Value.AdminDataProvider.Id, columns);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess("Data provider updated!");
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					AdminDataProvider = submitResult.Value,

				}));
				_keyitems = submitResult.Value.SynchPrimaryKeyColumns.ToList();
			}
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