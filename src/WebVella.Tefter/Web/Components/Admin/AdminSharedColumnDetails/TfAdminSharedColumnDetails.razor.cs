namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminSharedColumnDetails.TfAdminSharedColumnDetails", "WebVella.Tefter")]
public partial class TfAdminSharedColumnDetails : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private bool _isDeleting = false;
	private async Task _editColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSharedColumnManageDialog>(
		TfAppState.Value.AdminSharedColumn,
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
			var item = (TucSharedColumn)result.Data;
			var newState = TfAppState.Value with { AdminSharedColumn = item };
			var index = TfAppState.Value.AdminSharedColumns.FindIndex(x => x.Id == item.Id);
			if (index > -1)
			{
				var items = TfAppState.Value.AdminSharedColumns.ToList();
				items[index] = item;
				newState = newState with { AdminSharedColumns = items };
			}
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: newState));
		}
	}

	private async Task _deleteColumn()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?") + "\r\n" + LOC("This will delete all related data too!")))
			return;
		try
		{
			_isDeleting = true;
			await InvokeAsync(StateHasChanged);
			UC.DeleteSharedColumn(TfAppState.Value.AdminSharedColumn.Id);
			ToastService.ShowSuccess(LOC("The column is successfully deleted!"));
			Navigator.NavigateTo(TfConstants.AdminSharedColumnsPageUrl, true);
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