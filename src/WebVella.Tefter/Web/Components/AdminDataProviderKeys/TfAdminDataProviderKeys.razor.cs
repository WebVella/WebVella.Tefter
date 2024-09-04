namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminDataProviderKeys.TfAdminDataProviderKeys","WebVella.Tefter")]
public partial class TfAdminDataProviderKeys : TfBaseComponent
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] protected IState<DataProviderAdminState> DataProviderDetailsState { get; set; }

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
	}
	private void On_GetDataProviderDetailsActionResult(DataProviderAdminChangedAction action)
	{
		StateHasChanged();
	}


	private async Task _editKey(TucDataProviderSharedKey key)
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderKeyManageDialog>(
				key,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge
				});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			ToastService.ShowSuccess(LOC("The key was successfully updated!"));
			Dispatcher.Dispatch(new SetDataProviderAdminAction(false, (TucDataProvider)result.Data));
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
				Dispatcher.Dispatch(new SetDataProviderAdminAction(false, (TucDataProvider)result.Value));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}

	}
}