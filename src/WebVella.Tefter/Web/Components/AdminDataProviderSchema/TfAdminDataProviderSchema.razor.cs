
using WebVella.Tefter.Web.Components.DataProviderColumnManageDialog;

namespace WebVella.Tefter.Web.Components.AdminDataProviderSchema;
public partial class TfAdminDataProviderSchema : TfBaseComponent
{
	[Inject] protected IState<DataProviderAdminState> DataProviderDetailsState { get; set; }

	private Dictionary<DatabaseColumnType, string> _typeNameDict = new();
	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			Dispatcher.Dispatch(new EmptyDataProviderAdminAction());
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		return base.DisposeAsyncCore(disposing);
	}
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_getProvider();
		throw new NotImplementedException();
		//foreach (var item in SystemState.Value.DataProviderColumnTypes)
		//{
		//	_typeNameDict[item.Type] = item.Name;
		//}

		Navigator.LocationChanged += Navigator_LocationChanged;
	}
	private void On_GetDataProviderDetailsActionResult(DataProviderAdminChangedAction action)
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
			ActionSubscriber.SubscribeToAction<DataProviderAdminChangedAction>(this, On_GetDataProviderDetailsActionResult);
			Dispatcher.Dispatch(new GetDataProviderAdminAction(urlData.DataProviderId.Value));
		}
	}
	private async Task _editColumn(TfDataProviderColumn column)
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderColumnManageDialog>(
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
			var record = (TfDataProvider)result.Data;
			ToastService.ShowSuccess(LOC("Column successfully updated!"));
			Dispatcher.Dispatch(new SetDataProviderAdminAction(record));
		}
	}

	private async Task _deleteColumn(TfDataProviderColumn column)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?") + "\r\n" + LOC("This will delete all related data too!")))
			return;
		try
		{
			Result<TfDataProvider> result = DataProviderManager.DeleteDataProviderColumn(column.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("The column is successfully deleted!"));
				Dispatcher.Dispatch(new SetDataProviderAdminAction(result.Value));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		
	}
}