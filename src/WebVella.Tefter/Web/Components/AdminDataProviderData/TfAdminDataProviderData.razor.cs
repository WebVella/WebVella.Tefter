﻿
namespace WebVella.Tefter.Web.Components.AdminDataProviderData;
public partial class TfAdminDataProviderData : TfBaseComponent
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


	private async Task _editKey(TfDataProviderColumn column)
	{
		//var dialog = await DialogService.ShowDialogAsync<TfDataProviderColumnManageDialog>(
		//		new Tuple<TfDataProviderColumn, TfDataProvider>(column, DataProviderDetailsState.Value.Provider),
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
		//	ToastService.ShowSuccess(LOC("The key was successfully updated!"));
		//	Dispatcher.Dispatch(new SetDataProviderDetailsAction(record));
		//}
	}

	private async Task _deleteKey(TfDataProviderColumn column)
	{
		//if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this key deleted?")))
		//	return;
		//try
		//{
		//	Result<TfDataProvider> result = DataProviderManager.DeleteDataProviderColumn(column.Id);
		//	ProcessServiceResponse(result);
		//	if (result.IsSuccess)
		//	{
		//		ToastService.ShowSuccess(LOC("The key is successfully deleted!"));
		//		Dispatcher.Dispatch(new SetDataProviderAdminAction(result.Value));
		//	}
		//}
		//catch (Exception ex)
		//{
		//	ProcessException(ex);
		//}
		
	}
}