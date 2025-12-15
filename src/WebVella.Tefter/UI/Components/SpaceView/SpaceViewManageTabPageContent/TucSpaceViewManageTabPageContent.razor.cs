namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewManageTabPageContent : TfBaseComponent, IDisposable
{
	#region << Init >>
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	[Parameter] public TfSpacePageAddonContext? Context { get; set; } = null;

	// State
	private bool _isDataLoading = true;
	private TfNavigationState _navState = null!;
	private TfSpaceView? _spaceView = null;
	private TfDataset? _spaceData = null;
	private List<TfSpaceViewColumn> _spaceViewColumns = new();
	private ReadOnlyDictionary<Guid, ITfSpaceViewColumnTypeAddon> _typeMetaDict = null!;
	public bool _submitting = false;
	public Dictionary<Guid,TfRole> _roleDict = new();
	public List<TfRole> _rolesTotal = new();
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider.Dispose();
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Context is null)
			throw new Exception("Context cannot be null");
		_roleDict = TfService.GetRoles().ToDictionary(x=> x.Id);
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
		_isDataLoading = true;
		await InvokeAsync(StateHasChanged);
		_navState = navState;
		try
		{
			_spaceView = null;
			var options =
				JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(Context!.SpacePage.ComponentOptionsJson);
			if (options is null || options.SpaceViewId is null)
				return;
			_spaceView = TfService.GetSpaceView(options.SpaceViewId.Value);
			if(_spaceView is null) throw new Exception("View no longer exists");
			_spaceViewColumns = TfService.GetSpaceViewColumnsList(_spaceView.Id);
			_spaceData = TfService.GetDataset(_spaceView.DatasetId);
			if (_spaceData is null) throw new Exception("Dataset no longer exists");
			_typeMetaDict = TfMetaService.GetSpaceViewColumnTypeDictionary();

			_rolesTotal.Clear();
			var totalIds = _spaceView.Settings.CanCreateRoles.ToList();
			totalIds.AddRange(_spaceView.Settings.CanUpdateRoles.ToList());
			totalIds.AddRange(_spaceView.Settings.CanDeleteRoles.ToList());
			foreach (var roleId in totalIds)
			{
				if(roleId == TfConstants.ADMIN_ROLE_ID) continue;
				if(!_roleDict.ContainsKey(roleId)) continue;
				if (_rolesTotal.Any(x=> x.Id == roleId)) continue;

				_rolesTotal.Add(_roleDict[roleId]);
			}
			_rolesTotal = _rolesTotal.OrderBy(x=> x.Name).ToList();

		}
		finally
		{
			_isDataLoading = false;
			UriInitialized = _navState?.Uri ?? String.Empty;
			await InvokeAsync(StateHasChanged);
		}
	}
	
	private async Task _editSpaceView()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewManageDialog>(
			_spaceView!,
			new ()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null }) { }
	}	

	private async Task _importColumns()
	{
		if (_submitting) return;
		try
		{
			_submitting = true;
			var (added, viewColumns) = await TfService.ImportMissingColumnsFromDataset(_spaceView!.Id);
			if (!added)
			{
				ToastService.ShowSuccess(LOC("No new columns found in dataset"));
				return;
			}
			_spaceViewColumns =  viewColumns;
			ToastService.ShowSuccess(LOC("View columns were added!"));
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

	private async Task _addColumn()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewColumnManageDialog>(
			new TfSpaceViewColumn() with { SpaceViewId = _spaceView!.Id },
			new ()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null })
		{
		}
	}

	private async Task _editColumn(TfSpaceViewColumn column)
	{

		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewColumnManageDialog>(
			column,
			new ()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null })
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
			// await TfService.UpdateSpaceViewPresets(
			// 	spaceViewId: _spaceView!.Id,
			// 	presets: presets);
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