namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderKeys.TfAdminDataProviderKeys", "WebVella.Tefter")]
public partial class TfAdminDataProviderKeys : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private async Task _editKey(TucDataProviderSharedKey key)
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderKeyManageDialog>(
				key,
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
			ToastService.ShowSuccess(LOC("The key was successfully updated!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = (TucDataProvider)result.Data }));
		}
	}

	private async Task _deleteKey(TucDataProviderSharedKey key)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this key deleted?")))
			return;
		try
		{
			Result<TucDataProvider> result = UC.DeleteDataProviderSharedKey(key.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("The key is successfully deleted!"));
				Dispatcher.Dispatch(new SetAppStateAction(component: this,
					state: TfAppState.Value with { AdminDataProvider = (TucDataProvider)result.Value }));

			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}

	}
}