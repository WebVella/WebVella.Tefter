namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewManage : TfBaseComponent
{
	[Inject] protected IState<TfState> TfState { get; set; }

	[Inject] private SpaceUseCase UC { get; set; }
	private bool _isSubmitting = false;
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}
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
			var columns = await UC.InitSpaceViewManageAfterRender(TfState.Value);
			Dispatcher.Dispatch(new SetSpaceViewMetaAction(
				component: this,
				spaceViewColumns: columns
			));
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _addColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewColumnManageDialog>(
				new TucSpaceViewColumn() with { SpaceViewId = TfState.Value.RouteSpaceViewId.Value },
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge
				});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var updatedColumn = (TucSpaceViewColumn)result.Data;
			var columns = TfState.Value.SpaceViewColumns.ToList();
			columns.Add(updatedColumn);
			columns = columns.OrderBy(x => x.QueryName).ToList();

			Dispatcher.Dispatch(new SetSpaceViewMetaAction(
			component: this,
			spaceViewColumns: columns
			));
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editColumn(TucSpaceViewColumn column)
	{

		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewColumnManageDialog>(
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
			var updatedColumn = (TucSpaceViewColumn)result.Data;
			var columns = TfState.Value.SpaceViewColumns.ToList();

			var columnIndex = columns.FindIndex(x => x.Id == updatedColumn.Id);
			if (columnIndex > -1) columns[columnIndex] = updatedColumn;
			columns = columns.OrderBy(x => x.QueryName).ToList();
			Dispatcher.Dispatch(new SetSpaceViewMetaAction(
			component: this,
			spaceViewColumns: columns
			));
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _deleteColumn(TucSpaceViewColumn column)
	{
		if (_isSubmitting) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?")))
			return;

		try
		{
			_isSubmitting = true;
			Result submitResult = UC.RemoveSpaceViewColumn(column.Id);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space View updated!"));
				var columns = TfState.Value.SpaceViewColumns.ToList();
				var columnIndex = columns.FindIndex(x => x.Id == column.Id);
				if (columnIndex > -1)
				{
					columns.RemoveAt(columnIndex);
					Dispatcher.Dispatch(new SetSpaceViewMetaAction(
					component: this,
					spaceViewColumns: columns
					));
				}
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}

	}

	private async Task _moveColumn(TucSpaceViewColumn column, bool isUp)
	{
		if (_isSubmitting) return;
		try
		{
			Result<List<TucSpaceViewColumn>> submitResult = UC.MoveSpaceViewColumn(viewId: TfState.Value.SpaceView.Id, columnId: column.Id, isUp: isUp);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space View updated!"));

				Dispatcher.Dispatch(new SetSpaceViewMetaAction(
				component: this,
				spaceViewColumns: submitResult.Value
				));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}

	}

}