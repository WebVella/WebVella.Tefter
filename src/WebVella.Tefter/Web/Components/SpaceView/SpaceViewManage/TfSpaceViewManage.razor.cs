namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewManage.TfSpaceViewManage", "WebVella.Tefter")]
public partial class TfSpaceViewManage : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	private bool _isSubmitting = false;
	private TucSpaceViewPreset _spaceViewPreset = null;
	private TucSpaceData _spaceData = null;
	private TucDataProvider _dataProvider = null;
	private string _activeTab;


	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (TfAppState.Value.SpaceView is not null)
		{
			_spaceData = TfAppState.Value.SpaceDataList.FirstOrDefault(x => x.Id == TfAppState.Value.SpaceView.SpaceDataId);
			_dataProvider = TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == _spaceData?.DataProviderId);
			_spaceViewPreset = TfAppState.Value.SpaceView.Presets.FirstOrDefault(x => x.Id == TfAppState.Value.SpaceView.Id);
			if (_spaceViewPreset is null)
				_spaceViewPreset = new TucSpaceViewPreset { Id = TfAppState.Value.SpaceView.Id, Name = TfAppState.Value.SpaceView.Name };
		}
		_activeTab = (Navigator.GetEnumFromQuery<TfSpaceViewManageTab>(TfConstants.TabQueryName,TfSpaceViewManageTab.Columns).Value).ToString();
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

	private async Task _onPresetsChanged(List<TucSpaceViewPreset> presets)
	{
		if (_isSubmitting) return;
		try
		{
			Result<TucSpaceView> submitResult = UC.UpdateSpaceViewPresets(
				viewId: TfAppState.Value.SpaceView.Id,
				presets: presets);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space View updated!"));
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					SpaceView = submitResult.Value
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

	private async Task _tabChanged(FluentTab tabObj){ 
		TfSpaceViewManageTab tab = TfSpaceViewManageTab.Columns;
		if(Enum.TryParse<TfSpaceViewManageTab>(tabObj.Id,false,out tab)){ }
		var queryDict = new Dictionary<string,object>();
		queryDict[TfConstants.TabQueryName] = tab.ToString();
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
}

internal enum TfSpaceViewManageTab{ 
	Columns = 0,
	QuickFilters = 1
}