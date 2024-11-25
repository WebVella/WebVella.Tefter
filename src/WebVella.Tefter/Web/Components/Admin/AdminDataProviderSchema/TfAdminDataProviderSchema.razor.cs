namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderSchema.TfAdminDataProviderSchema", "WebVella.Tefter")]
public partial class TfAdminDataProviderSchema : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private Guid? _deletedColumnId = null;
	private async Task _editColumn(TucDataProviderColumn column)
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderColumnManageDialog>(
				column,
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
			ToastService.ShowSuccess(LOC("Column successfully updated!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminDataProvider = (TucDataProvider)result.Data }));

		}
	}

	private async Task _deleteColumn(TucDataProviderColumn column)
	{
		if (_deletedColumnId is not null) return;
		_deletedColumnId = column.Id;

		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?") + "\r\n" + LOC("This will delete all related data too!")))
		{
			_deletedColumnId = null;
			return;
		}
		await InvokeAsync(StateHasChanged);
		try
		{
			Result<TucDataProvider> result = UC.DeleteDataProviderColumn(column.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("The column is successfully deleted!"));
				Dispatcher.Dispatch(new SetAppStateAction(component: this,
					state: TfAppState.Value with { AdminDataProvider = result.Value }));

			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally{
			_deletedColumnId = null;
			await InvokeAsync(StateHasChanged);
		}
	}
}