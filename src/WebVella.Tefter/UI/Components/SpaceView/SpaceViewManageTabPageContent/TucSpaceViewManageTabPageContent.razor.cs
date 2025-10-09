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
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider.SpaceViewUpdatedEvent -= On_SpaceViewUpdated;
		TfEventProvider.SpaceViewColumnsChangedEvent -= On_SpaceViewColumnUpdated;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Context is null)
			throw new Exception("Context cannot be null");
		await _init(TfAuthLayout.GetState().NavigationState);
		_isDataLoading = false;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			Navigator.LocationChanged += On_NavigationStateChanged;
			TfEventProvider.SpaceViewUpdatedEvent += On_SpaceViewUpdated;
			TfEventProvider.SpaceViewColumnsChangedEvent += On_SpaceViewColumnUpdated;
		}
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
			{
				await _init(TfAuthLayout.GetState().NavigationState);
			}
		});
	}

	private async Task On_SpaceViewUpdated(TfSpaceViewUpdatedEvent args)
	{
		if(args.UserId != TfAuthLayout.GetState().User.Id) return;
		await _init(TfAuthLayout.GetState().NavigationState);
	}	
	private async Task On_SpaceViewColumnUpdated(TfSpaceViewColumnsChangedEvent args)
	{
		if(args.UserId != TfAuthLayout.GetState().User.Id) return;
		await _init(TfAuthLayout.GetState().NavigationState);
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
			_spaceView = TfService.GetSpaceView(options.SpaceViewId.Value);
			_spaceViewColumns = TfService.GetSpaceViewColumnsList(_spaceView.Id);
			_spaceData = TfService.GetDataset(_spaceView.DatasetId);
			if (_spaceData is null) throw new Exception("Dataset no longer exists");
			_typeMetaDict = TfMetaService.GetSpaceViewColumnTypeMetaDictionary();
			_componentMetaDict = TfMetaService.GetSpaceViewColumnComponentMetaDictionary();			
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

	private async Task _editMainTab()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewManageMainTabDialog>(
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
			await TfService.DeleteSpaceViewColumn(column.Id);
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
			if(isUp)
				await TfService.MoveSpaceViewColumnUp(column.Id);
			else
				await TfService.MoveSpaceViewColumnDown(column.Id);
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
			await TfService.UpdateSpaceViewPresets(
				spaceViewId: _spaceView.Id,
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