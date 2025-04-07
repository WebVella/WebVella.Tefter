namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewManage.TfSpaceViewManage", "WebVella.Tefter")]
public partial class TfSpaceViewManage : TfBaseComponent
{
	[Parameter] public string Menu { get; set; } = "";
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	private bool _isSubmitting = false;
	private TucSpaceViewPreset _spaceViewPreset = null;
	private TucSpaceData _spaceData { get => TfAppState.Value.SpaceDataList.FirstOrDefault(x => x.Id == TfAppState.Value.SpaceView.SpaceDataId); }
	private TucDataProvider _dataProvider { get => TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == _spaceData?.DataProviderId); }
	private string _activeTab;


	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
		if (TfAppState.Value.SpaceView is not null)
		{
			_spaceViewPreset = TfAppState.Value.SpaceView.Presets.FirstOrDefault(x => x.Id == TfAppState.Value.SpaceView.Id);
			if (_spaceViewPreset is null)
				_spaceViewPreset = new TucSpaceViewPreset { Id = TfAppState.Value.SpaceView.Id, Name = TfAppState.Value.SpaceView.Name };
		}
		_activeTab = (Navigator.GetEnumFromQuery<TfSpaceViewManageTab>(TfConstants.TabQueryName, TfSpaceViewManageTab.Columns).Value).ToString();
	}

	private async Task _deleteSpaceView()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space view deleted?")))
			return;
		try
		{
			UC.DeleteSpaceView(TfAppState.Value.SpaceView.Id);

			ToastService.ShowSuccess(LOC("Space view deleted"));

			if (TfAppState.Value.SpaceViewList.Count > 0)
				Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceViewList[0].Id), true);
			else
				Navigator.NavigateTo(String.Format(TfConstants.SpacePageUrl, TfAppState.Value.Space.Id), true);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}

	}

	private async Task _editSpaceView()
	{

		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewManageDialog>(
		TfAppState.Value.SpaceView,
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
			var resultObj = (Tuple<TucSpaceView, TucSpaceData>)result.Data;
			var spaceView = resultObj.Item1;
			var spaceData = resultObj.Item2;
			var viewList = TfAppState.Value.SpaceViewList.ToList();
			var dataList = TfAppState.Value.SpaceDataList.ToList();
			var viewIndex = viewList.FindIndex(x => x.Id == spaceView.Id);
			if (viewIndex > -1)
			{
				viewList[viewIndex] = spaceView;
			}

			var dataIndex = dataList.FindIndex(x => x.Id == spaceData.Id);
			if (dataIndex > -1)
			{
				dataList[dataIndex] = spaceData;
			}
			else
			{
				dataList.Add(spaceData);
			}

			Dispatcher.Dispatch(new SetAppStateAction(
						component: this,
						state: TfAppState.Value with
						{
							SpaceView = spaceView,
							SpaceViewList = viewList.OrderBy(x => x.Position).ToList(),
							SpaceDataList = dataList.OrderBy(x => x.Position).ToList()
						}));

			ToastService.ShowSuccess(LOC("Space view successfully updated!"));
		}
	}

	private async Task _addColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewColumnManageDialog>(
				new TucSpaceViewColumn() with { SpaceViewId = TfAppState.Value.SpaceView.Id },
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
					Width = TfConstants.DialogWidthLarge,
					TrapFocus = false
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
			List<TucSpaceViewColumn> submitResult = UC.RemoveSpaceViewColumn(column.Id);
			ToastService.ShowSuccess(LOC("Space View updated!"));

			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with { SpaceViewColumns = submitResult }
			));
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
			var submitResult = UC.MoveSpaceViewColumn(viewId: TfAppState.Value.SpaceView.Id, columnId: column.Id, isUp: isUp);
			ToastService.ShowSuccess(LOC("Space View updated!"));
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceViewColumns = submitResult
			}
			));
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
			var submitResult = UC.UpdateSpaceViewPresets(
				viewId: TfAppState.Value.SpaceView.Id,
				presets: presets);

			ToastService.ShowSuccess(LOC("Space View updated!"));
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
				{
					SpaceView = submitResult
				}
			));
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

	private async Task _tabChanged(FluentTab tabObj)
	{
		TfSpaceViewManageTab tab = TfSpaceViewManageTab.Columns;
		if (Enum.TryParse<TfSpaceViewManageTab>(tabObj.Id, false, out tab)) { }
		var queryDict = new Dictionary<string, object>{
			{TfConstants.TabQueryName, tab.ToString()}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
}

internal enum TfSpaceViewManageTab
{
	Columns = 0,
	QuickFilters = 1
}