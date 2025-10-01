namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewManageTabPageContent : TfBaseComponent, IDisposable
{
	#region << Init >>

	[Parameter] public TfSpacePageAddonContext? Context { get; set; } = null;

	// State
	private bool _isDataLoading = true;
	private TfNavigationState _navState = null!;
	private TfSpaceView? _spaceView = null;
	private TfDataset? _spaceData = null;
	private List<TfSpaceViewColumn> _spaceViewColumns = new();
	private ReadOnlyDictionary<Guid, TfSpaceViewColumnTypeAddonMeta> _typeMetaDict = null!;
	private ReadOnlyDictionary<Guid, TfSpaceViewColumnComponentAddonMeta> _componentMetaDict = null!;
	public bool _submitting = false;
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUIService.SpaceViewColumnsChanged -= On_SpaceViewUpdated;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Context is null)
			throw new Exception("Context cannot be null");
		await _init(Navigator.GetRouteState());
		_isDataLoading = false;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			TfUIService.NavigationStateChanged += On_NavigationStateChanged;
			TfUIService.SpaceViewColumnsChanged += On_SpaceViewUpdated;
		}
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
			{
				await _init(args);
			}
		});
	}

	private async void On_SpaceViewUpdated(object? caller, List<TfSpaceViewColumn> args)
	{
		await _init(Navigator.GetRouteState());
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_isDataLoading = true;
			await InvokeAsync(StateHasChanged);
			_navState = navState;
			_spaceView = null;
			var options =
				JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(Context!.SpacePage.ComponentOptionsJson);
			if (options is null || options.SpaceViewId is null)
				return;
			_spaceView = TfUIService.GetSpaceView(options.SpaceViewId.Value);
			_spaceViewColumns = TfUIService.GetViewColumns(_spaceView.Id);
			_spaceData = TfUIService.GetDataset(_spaceView.DatasetId);
			if (_spaceData is null) throw new Exception("Dataset no longer exists");
			_typeMetaDict = TfUIService.GetSpaceViewColumnTypeDict();
			_componentMetaDict = TfUIService.GetSpaceViewColumnComponentDict();			
		}
		finally
		{
			_isDataLoading = false;
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
	
	private async Task _editSpaceView()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewManageDialog>(
			_spaceView,
			new DialogParameters()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
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
	
	private async Task _onPresetsChanged(List<TfSpaceViewPreset> presets)
	{
		if (_submitting) return;
		try
		{
			TfUIService.UpdateSpaceViewPresets(
				viewId: _spaceView.Id,
				presets: presets);
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
	
	#endregion
}