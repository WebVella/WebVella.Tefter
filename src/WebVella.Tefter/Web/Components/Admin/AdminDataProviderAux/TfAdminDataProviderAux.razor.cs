namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderAux.TfAdminDataProviderAux", "WebVella.Tefter")]
public partial class TfAdminDataProviderAux : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private async Task _addKey()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderKeyManageDialog>(
		new TucDataProviderSharedKey(),
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
			ToastService.ShowSuccess(LOC("Key successfully created!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = record }));
		}
	}

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
			var provider = UC.DeleteDataProviderSharedKey(key.Id);
			ToastService.ShowSuccess(LOC("The key is successfully deleted!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = (TucDataProvider)provider }));

		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}

	}

}