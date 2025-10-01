namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewColumnsContent : TfBaseComponent, IDisposable
{
	private TfSpaceView _spaceView = new();
	private TfDataset _spaceData = new();
	private List<TfSpaceViewColumn> _spaceViewColumns = new();
	private TfSpace _space = new();
	public bool _submitting = false;
	public TfNavigationState? _navState = null;
	private ReadOnlyDictionary<Guid, TfSpaceViewColumnTypeAddonMeta> _typeMetaDict = null!;
	private ReadOnlyDictionary<Guid, TfSpaceViewColumnComponentAddonMeta> _componentMetaDict = null!;
	public void Dispose()
	{
		TfUIService.SpaceViewUpdated -= On_SpaceViewUpdated;
		TfUIService.SpaceViewColumnsChanged -= On_SpaceViewColumnsUpdated;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(Navigator.GetRouteState());
		TfUIService.SpaceViewUpdated += On_SpaceViewUpdated;
		TfUIService.SpaceViewColumnsChanged += On_SpaceViewColumnsUpdated;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_SpaceViewUpdated(object? caller, TfSpaceView args)
	{
		await _init(navState:Navigator.GetRouteState(), spaceView: args);
	}

	private async void On_SpaceViewColumnsUpdated(object? caller, List<TfSpaceViewColumn> args)
	{
		await _init(Navigator.GetRouteState());
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState navState, TfSpaceView? spaceView = null)
	{
		_navState = navState;
		try
		{
			if (spaceView is not null && spaceView.Id == _spaceView?.Id)
			{
				_spaceView = spaceView;
			}
			else
			{
				var routeData = Navigator.GetRouteState();
				if (routeData.SpaceViewId is not null)
					_spaceView = TfUIService.GetSpaceView(routeData.SpaceViewId.Value);

			}
			if (_spaceView is null) return;
			_space = TfUIService.GetSpace(_spaceView.SpaceId);
			_spaceViewColumns = TfUIService.GetViewColumns(_spaceView.Id);
			_spaceData = TfUIService.GetDataset(_spaceView.DatasetId);
			_typeMetaDict = TfUIService.GetSpaceViewColumnTypeDict();
			_componentMetaDict = TfUIService.GetSpaceViewColumnComponentDict();
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _addColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewColumnManageDialog>(
				new TfSpaceViewColumn() with { SpaceViewId = _spaceView.Id },
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
		}
	}

	private async Task _editColumn(TfSpaceViewColumn column)
	{

		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewColumnManageDialog>(
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
		}
	}

	private async Task _deleteColumn(TfSpaceViewColumn column)
	{
		if (_submitting) return;
		//if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?")))
		//	return;

		try
		{
			_submitting = true;
			TfUIService.RemoveSpaceViewColumn(column.Id);
			ToastService.ShowSuccess(LOC("Space View updated!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			await InvokeAsync(StateHasChanged);
		}

	}

	private async Task _moveColumn(TfSpaceViewColumn column, bool isUp)
	{
		if (_submitting) return;
		try
		{
			TfUIService.MoveSpaceViewColumn(viewId: _spaceView.Id, columnId: column.Id, isUp: isUp);
			ToastService.ShowSuccess(LOC("Space View updated!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			await InvokeAsync(StateHasChanged);
		}

	}

}