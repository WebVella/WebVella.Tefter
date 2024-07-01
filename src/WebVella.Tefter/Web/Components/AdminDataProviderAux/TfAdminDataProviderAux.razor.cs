
using WebVella.Tefter.Web.Components.DataProviderColumnManageDialog;

namespace WebVella.Tefter.Web.Components.AdminDataProviderAux;
public partial class TfAdminDataProviderAux : TfBaseComponent
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
	protected override void OnInitialized()
	{
		base.OnInitialized();
	}
	private void On_GetDataProviderDetailsActionResult(DataProviderAdminChangedAction action)
	{
		StateHasChanged();
	}

	private async Task _editColumn(TfDataProviderColumn column)
	{
		//var dialog = await DialogService.ShowDialogAsync<TfDataProviderColumnManageDialog>(
		//		column,
		//		new DialogParameters()
		//		{
		//			PreventDismissOnOverlayClick = true,
		//			PreventScroll = true,
		//			Width = TfConstants.DialogWidthLarge
		//		});
		//var result = await dialog.Result;
		//if (!result.Cancelled && result.Data != null)
		//{
		//	var record = (TfDataProvider)result.Data;
		//	ToastService.ShowSuccess(LOC("Column successfully updated!"));
		//	Dispatcher.Dispatch(new SetDataProviderAdminAction(record));
		//}
	}

	private async Task _deleteColumn(TfDataProviderColumn column)
	{
		//if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?") + "\r\n" + LOC("This will delete all related data too!")))
		//	return;
		//try
		//{
		//	Result<TfDataProvider> result = DataProviderManager.DeleteDataProviderColumn(column.Id);
		//	ProcessServiceResponse(result);
		//	if (result.IsSuccess)
		//	{
		//		ToastService.ShowSuccess(LOC("The column is successfully deleted!"));
		//		Dispatcher.Dispatch(new SetDataProviderAdminAction(result.Value));
		//	}
		//}
		//catch (Exception ex)
		//{
		//	ProcessException(ex);
		//}
		
	}
}