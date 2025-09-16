namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewColumnsContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] public ITfMetaUIService TfMetaUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private TfSpaceView _spaceView = new();
	private TfSpaceData _spaceData = new();
	private List<TfSpaceViewColumn> _spaceViewColumns = new();
	private TfSpace _space = new();
	public bool _submitting = false;
	public TfNavigationState? _navState = null;
	private ReadOnlyDictionary<Guid, TfSpaceViewColumnTypeAddonMeta> _typeMetaDict = default!;
	private ReadOnlyDictionary<Guid, TfSpaceViewColumnComponentAddonMeta> _componentMetaDict = default!;
	public void Dispose()
	{
		TfSpaceViewUIService.SpaceViewUpdated -= On_SpaceViewUpdated;
		TfSpaceViewUIService.SpaceViewColumnsChanged -= On_SpaceViewColumnsUpdated;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfSpaceViewUIService.SpaceViewUpdated += On_SpaceViewUpdated;
		TfSpaceViewUIService.SpaceViewColumnsChanged += On_SpaceViewColumnsUpdated;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_SpaceViewUpdated(object? caller, TfSpaceView args)
	{
		await _init(spaceView: args);
	}

	private async void On_SpaceViewColumnsUpdated(object? caller, List<TfSpaceViewColumn> args)
	{
		await _init(null);
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState? navState = null, TfSpaceView? spaceView = null)
	{
		if (navState == null)
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		else
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
					_spaceView = TfSpaceViewUIService.GetSpaceView(routeData.SpaceViewId.Value);

			}
			if (_spaceView is null) return;
			_space = TfSpaceUIService.GetSpace(_spaceView.SpaceId);
			_spaceViewColumns = TfSpaceViewUIService.GetViewColumns(_spaceView.Id);
			_spaceData = TfSpaceDataUIService.GetSpaceData(_spaceView.SpaceDataId);
			_typeMetaDict = TfMetaUIService.GetSpaceViewColumnTypeDict();
			_componentMetaDict = TfMetaUIService.GetSpaceViewColumnComponentDict();
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
			TfSpaceViewUIService.RemoveSpaceViewColumn(column.Id);
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
			TfSpaceViewUIService.MoveSpaceViewColumn(viewId: _spaceView.Id, columnId: column.Id, isUp: isUp);
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