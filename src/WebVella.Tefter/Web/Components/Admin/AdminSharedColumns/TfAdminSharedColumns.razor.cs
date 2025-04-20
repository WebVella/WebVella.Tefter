namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminSharedColumns.TfAdminSharedColumns", "WebVella.Tefter")]
public partial class TfAdminSharedColumns : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	private string _search = null;
	private async Task _addColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSharedColumnManageDialog>(
		new TucSharedColumn(),
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
			ToastService.ShowSuccess(LOC("Column successfully created!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminSharedColumns = (List<TucSharedColumn>)result.Data }));
		}
	}

	private async Task _editColumn(TucSharedColumn column)
	{
		var dialog = await DialogService.ShowDialogAsync<TfSharedColumnManageDialog>(
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
				state: TfAppState.Value with { AdminSharedColumns = (List<TucSharedColumn>)result.Data }));
		}
	}

	private async Task _deleteColumn(TucSharedColumn column)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?") + "\r\n" + LOC("This will delete all related data too!")))
			return;
		try
		{
			var result = UC.DeleteSharedColumn(column.Id);
			ToastService.ShowSuccess(LOC("The column is successfully deleted!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminSharedColumns = (List<TucSharedColumn>)result }));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}

	}

	private async Task _searchValueChanged(string search)
	{
		_search = search?.Trim();
		await InvokeAsync(StateHasChanged);
	}

	private List<TucSharedColumn> _getColumns()
	{
		string searchProcessed = null;
		if (!String.IsNullOrWhiteSpace(_search))
			searchProcessed = _search.Trim().ToLowerInvariant();

		if (!String.IsNullOrWhiteSpace(searchProcessed))
		{
			return TfAppState.Value.AdminSharedColumns.Where(x =>
				x.DbName.ToLowerInvariant().Contains(searchProcessed)
				|| x.JoinKeyDbName.ToLowerInvariant().Contains(searchProcessed)
			).ToList();
		}
		else
		{
			return TfAppState.Value.AdminSharedColumns;
		}

	}
}