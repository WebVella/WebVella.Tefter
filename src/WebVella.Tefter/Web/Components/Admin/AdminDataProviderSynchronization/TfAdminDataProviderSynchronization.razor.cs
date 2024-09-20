namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderSynchronization.TfAdminDataProviderSynchronization","WebVella.Tefter")]
public partial class TfAdminDataProviderSynchronization : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	private bool _isSynchronizing = false;
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
		await UC.TriggerSynchronization(TfAppState.Value.AdminDataProvider.Id);
		ToastService.ShowSuccess(LOC("Synchronization task created!"));
		Navigator.ReloadCurrentUrl();
	}
}