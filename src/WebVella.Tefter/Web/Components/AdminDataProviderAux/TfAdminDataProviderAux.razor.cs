
using WebVella.Tefter.Web.Components.DataProviderAuxColumnManageDialog;

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
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		ActionSubscriber.SubscribeToAction<DataProviderAdminChangedAction>(this, On_GetDataProviderDetailsActionResult);
	}
	private void On_GetDataProviderDetailsActionResult(DataProviderAdminChangedAction action)
	{
		StateHasChanged();
	}

	private async Task _editColumn(TucDataProviderAuxColumn column)
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderAuxColumnManageDialog>(
		column,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			ToastService.ShowSuccess(LOC("Column successfully updated!"));
			Dispatcher.Dispatch(new SetDataProviderAdminAction(false, (TucDataProvider)result.Data));
		}
	}

	private async Task _deleteColumn(TfDataProviderColumn column)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?") + "\r\n" + LOC("This will delete all related data too!")))
			return;
		try
		{
			Result<TucDataProvider> result = UC.DeleteDataProviderAuxColumn(column.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("The column is successfully deleted!"));
				Dispatcher.Dispatch(new SetDataProviderAdminAction(false,result.Value));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}

	}
}