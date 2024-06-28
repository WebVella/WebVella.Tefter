
namespace WebVella.Tefter.Web.Components.AdminDataProviderKeys;
public partial class TfAdminDataProviderKeys : TfBaseComponent
{
	[Inject] protected IState<DataProviderDetailsState> DataProviderDetailsState { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			Dispatcher.Dispatch(new EmptyDataProviderDetailsAction());
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		return base.DisposeAsyncCore(disposing);
	}
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_getProvider();
		Navigator.LocationChanged += Navigator_LocationChanged;
	}
	private void On_GetDataProviderDetailsActionResult(DataProviderDetailsChangedAction action)
	{
		StateHasChanged();
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		_getProvider();
	}

	private void _getProvider()
	{
		var urlData = Navigator.GetUrlData();
		if (urlData.DataProviderId is not null)
		{
			ActionSubscriber.SubscribeToAction<DataProviderDetailsChangedAction>(this, On_GetDataProviderDetailsActionResult);
			Dispatcher.Dispatch(new GetDataProviderDetailsAction(urlData.DataProviderId.Value));
		}
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
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this key deleted?")))
			return;
		try
		{
			Result<TfDataProvider> result = DataProviderManager.DeleteDataProviderColumn(column.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("The key is successfully deleted!"));
				Dispatcher.Dispatch(new SetDataProviderDetailsAction(result.Value));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		
	}
}