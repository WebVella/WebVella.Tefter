namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewManage : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	private bool _isSubmitting = false;
	private TucSpaceData _spaceData = null;
	private TucDataProvider _dataProvider = null;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_spaceData = TfAppState.Value.SpaceDataList.FirstOrDefault(x => x.Id == TfAppState.Value.SpaceView.SpaceDataId);
		_dataProvider = TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == _spaceData.DataProviderId);

	}

	private async Task _addColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewColumnManageDialog>(
				new TucSpaceViewColumn() with { SpaceViewId = TfAppState.Value.SpaceView.Id },
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge
				});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var updatedResult = (List<TucSpaceViewColumn>)result.Data;
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with { SpaceViewColumns = updatedResult }
			));
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
			var updatedResult = (List<TucSpaceViewColumn>)result.Data;
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with { SpaceViewColumns = updatedResult }
			));
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
			Result<List<TucSpaceViewColumn>> submitResult = UC.RemoveSpaceViewColumn(column.Id);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space View updated!"));

				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with { SpaceViewColumns = submitResult.Value }
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

	private async Task _moveColumn(TucSpaceViewColumn column, bool isUp)
	{
		if (_isSubmitting) return;
		try
		{
			Result<List<TucSpaceViewColumn>> submitResult = UC.MoveSpaceViewColumn(viewId: TfAppState.Value.SpaceView.Id, columnId: column.Id, isUp: isUp);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space View updated!"));
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					SpaceViewColumns = submitResult.Value
				}
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