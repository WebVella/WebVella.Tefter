namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminDataProviderSynchronization.TfAdminDataProviderSynchronization","WebVella.Tefter")]
public partial class TfAdminDataProviderSynchronization : TfBaseComponent
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] protected IState<DataProviderAdminState> DataProviderDetailsState { get; set; }

	PaginationState _pagination = new PaginationState { ItemsPerPage = 15 };

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		_pagination.ItemsPerPage = UC.SyncLogPageSize;
		ActionSubscriber.SubscribeToAction<DataProviderAdminChangedAction>(this, On_GetDataProviderDetailsActionResult);
	}

	private void On_GetDataProviderDetailsActionResult(DataProviderAdminChangedAction action)
	{
		if (action.Provider is null) return;
		base.InvokeAsync(async () =>
		{
			UC.IsBusy = true;
			await InvokeAsync(StateHasChanged);
			await UC.LoadDataProviderDataObjects(action.Provider.Id);
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		});

	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await UC.LoadDataProviderDataObjects(DataProviderDetailsState.Value.Provider.Id);
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		}
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
					Width = TfConstants.DialogWidthExtraLarge
				});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{

		}

	}


	private async Task _synchronizeNow()
	{
		if (UC.IsSynchronizing) return;
		UC.IsSynchronizing = true;
		await InvokeAsync(StateHasChanged);
		await UC.TriggerSynchronization(DataProviderDetailsState.Value.Provider.Id);
		await UC.LoadDataProviderDataObjects(DataProviderDetailsState.Value.Provider.Id);
		await InvokeAsync(StateHasChanged);

	}
}