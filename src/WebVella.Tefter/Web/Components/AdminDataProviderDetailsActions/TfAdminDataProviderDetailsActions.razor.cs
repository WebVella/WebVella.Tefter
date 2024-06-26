using WebVella.Tefter.Web.Components.DataProviderManageDialog;

namespace WebVella.Tefter.Web.Components.AdminDataProviderDetailsActions;
public partial class TfAdminDataProviderDetailsActions : TfBaseComponent
{
	[Inject] protected IState<DataProviderDetailsState> DataProviderDetailsState { get; set; }

	private async Task _editProvider(){
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
}