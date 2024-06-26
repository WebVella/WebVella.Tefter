using WebVella.Tefter.Web.Components.DataProviderManageDialog;

namespace WebVella.Tefter.Web.Components.AdminDataProviderDetailsActions;
public partial class TfAdminDataProviderDetailsActions : TfBaseComponent
{
	[Inject] protected IState<DataProviderDetailsState> DataProviderDetailsState { get; set; }
	private bool _isDeleting = false;
	private async Task _editProvider()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderManageDialog>(DataProviderDetailsState.Value.Provider,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var record = (TfDataProvider)result.Data;
			ToastService.ShowSuccess(LOC("Provider successfully updated!"));
			Dispatcher.Dispatch(new SetDataProviderDetailsAction(record));
		}
	}

	private async Task _deleteProvider()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this data provider deleted?") + "\r\n" + LOC("Will proceeed only if there are not existing columns attached")))
			return;

		if (DataProviderDetailsState.Value is null || DataProviderDetailsState.Value.Provider is null) return;
		try
		{
			_isDeleting = true;
			await InvokeAsync(StateHasChanged);
			var result = DataProviderManager.DeleteDataProvider(DataProviderDetailsState.Value.Provider.Id);
			ProcessServiceResponse(result);
			if(result.IsSuccess) {
				Navigator.NavigateTo(TfConstants.AdminDataProvidersPageUrl);
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isDeleting = false;
			await InvokeAsync(StateHasChanged);
		}

	}
}