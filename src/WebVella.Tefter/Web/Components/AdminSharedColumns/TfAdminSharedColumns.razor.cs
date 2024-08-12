using WebVella.Tefter.UseCases.SharedColumnsAdmin;
using WebVella.Tefter.Web.Components.SharedColumnManageDialog;

namespace WebVella.Tefter.Web.Components.AdminSharedColumns;
public partial class TfAdminSharedColumns : TfBaseComponent
{
	[Inject] private SharedColumnsAdminUseCase UC { get; set; }
	[Inject] protected IState<DataProviderAdminState> DataProviderDetailsState { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await UC.LoadSharedColumnList();
			UC.IsBusy = false;
			UC.IsListBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void On_SharedColumnAdminChanged(TucSharedColumn column, Guid columnId)
	{
		var columnIndex = UC.SharedColumns.FindIndex(x => x.Id == columnId);
		//Create
		if (column is not null && columnIndex == -1)
		{
			UC.SharedColumns.Add(column);
			UC.SharedColumns = UC.SharedColumns.OrderBy(x => x.DbName).ToList();
		}
		//Edit
		else if (column is not null && columnIndex > -1)
		{
			UC.SharedColumns[columnIndex] = column;
		}
		//Delete
		else
		{
			UC.SharedColumns.RemoveAt(columnIndex);
		}

		StateHasChanged();
	}

	private async Task _addColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSharedColumnManageDialog>(
		new TucSharedColumn(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var record = (TucSharedColumn)result.Data;
			ToastService.ShowSuccess(LOC("Column successfully created!"));
			On_SharedColumnAdminChanged(record, record.Id);
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
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var record = (TucSharedColumn)result.Data;
			ToastService.ShowSuccess(LOC("Column successfully updated!"));
			On_SharedColumnAdminChanged(record, record.Id);
		}
	}

	private async Task _deleteColumn(TucSharedColumn column)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?") + "\r\n" + LOC("This will delete all related data too!")))
			return;
		try
		{
			Result<TucDataProvider> result = UC.DeleteSharedColumn(column.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("The column is successfully deleted!"));
				On_SharedColumnAdminChanged(null, column.Id);
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}

	}

	private async Task _searchValueChanged(string search)
	{
		search = search?.Trim();
		if (String.IsNullOrWhiteSpace(search)) search = null;

		await Navigator.SetParamToUrlQuery(TfConstants.WvSearchQuery, search);
		UC.Search = search;
		UC.IsListBusy = true;
		await UC.LoadSharedColumnList();
		UC.IsListBusy = false;
		await InvokeAsync(StateHasChanged);
	}
}