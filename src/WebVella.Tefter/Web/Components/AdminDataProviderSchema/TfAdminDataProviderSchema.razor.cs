namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminDataProviderSchema.TfAdminDataProviderSchema","WebVella.Tefter")]
public partial class TfAdminDataProviderSchema : TfBaseComponent
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] protected IState<TfState> TfState { get; set; }

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

	private async Task _editColumn(TucDataProviderColumn column)
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
			ToastService.ShowSuccess(LOC("Column successfully updated!"));
			Dispatcher.Dispatch(new SetDataProviderAdminAction(false, (TucDataProvider)result.Data));
		}
	}

	private async Task _deleteColumn(TucDataProviderColumn column)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?") + "\r\n" + LOC("This will delete all related data too!")))
			return;
		try
		{
			Result<TucDataProvider> result = UC.DeleteDataProviderColumn(column.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("The column is successfully deleted!"));
				Dispatcher.Dispatch(new SetDataProviderAdminAction(false, result.Value));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}

	}
}