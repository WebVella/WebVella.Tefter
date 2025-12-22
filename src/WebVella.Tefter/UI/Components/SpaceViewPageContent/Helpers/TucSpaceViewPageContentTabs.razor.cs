using ITfEventBus = WebVella.Tefter.UI.EventsBus.ITfEventBus;

namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContentTabs : TfBaseComponent, IAsyncDisposable
{
	// Dependency Injection
	[Inject] protected ITfEventBus TfEventBus { get; set; } = null!;
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfSpacePage SpacePage { get; set; } = null!;
	[Parameter] public TfDataset SpaceData { get; set; } = null!;

	private List<TfMenuItem> _menu = new();
	private TfNavigationState? _navState = null!;
	private Guid _initedViewId = Guid.Empty;
	private IAsyncDisposable _spaceViewUpdatedEventSubscriber = null!;
	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _spaceViewUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_init();
	}

	protected override void OnParametersSet()
	{
		if (_initedViewId != SpaceView.Id)
			_init();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			Navigator.LocationChanged += On_NavigationStateChanged;
			_spaceViewUpdatedEventSubscriber =
				await TfEventBus.SubscribeAsync<TfSpaceViewUpdatedEventPayload>(
					handler: On_SpaceViewUpdatedEventAsync);
		}
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
			{
				_init();
				await InvokeAsync(StateHasChanged);
			}
		});
	}

	private async Task On_SpaceViewUpdatedEventAsync(string? key, TfSpaceViewUpdatedEventPayload? payload)
	{
		if(payload is null) return;
		_init(newView: payload.SpaceView);
		await InvokeAsync(StateHasChanged);		
	}

	private void _init(TfSpaceView? newView = null)
	{
		_navState = TfAuthLayout.GetState().NavigationState;
		TfSpaceView spaceView = newView ?? SpaceView;
		try
		{
			_menu = new();
			string? pinnedDataIdentity = Navigator.GetStringFromQuery(TfConstants.DataIdentityIdQueryName, null);
			string? pinnedDataIdentityValue = Navigator.GetStringFromQuery(TfConstants.DataIdentityValueQueryName, null);
			var hasPinnedData = false;
			if (!String.IsNullOrWhiteSpace(pinnedDataIdentity) && !String.IsNullOrWhiteSpace(pinnedDataIdentityValue))
				hasPinnedData = true;

			Guid? currentPresetId = Navigator.GetGuidFromQuery(TfConstants.PresetIdQueryName);
			var first = true;
			if (spaceView.Presets.Count > 0)
			{
				if (currentPresetId is null && !hasPinnedData)
					currentPresetId = spaceView.Presets[0].Id;

				foreach (var prItem in spaceView.Presets)
				{
					_menu.Add(_getMenuItem(prItem, first, true));
					first = false;
				}
				if (!hasPinnedData)
				{
					foreach (var menuItem in _menu)
					{
						_setSelection(menuItem, currentPresetId);
					}
				}
			}

			if (hasPinnedData)
			{
				if (spaceView.Presets.Count == 0)
				{
					_menu.Add(new TfMenuItem
					{
						Id = "space-view-pinned-clear",
						Text = LOC("All Records"),
						Url = String.Format(TfConstants.SpacePagePageUrl, SpacePage.SpaceId, SpacePage.Id)
					});
				}
				_menu.Add(new TfMenuItem
				{
					Id = "space-view-pinned-data-tab",
					Text = LOC("Pinned"),
					Selected = true,
					IconColor = TfColor.Orange500,
					Url = String.Format(TfConstants.SpacePagePageUrl, SpacePage.SpaceId, SpacePage.Id)
						.ApplyChangeToUrlQuery(TfConstants.DataIdentityIdQueryName, pinnedDataIdentity)
						.ApplyChangeToUrlQuery(TfConstants.DataIdentityValueQueryName, pinnedDataIdentityValue),
					IconCollapsed = TfConstants.GetIcon("Pin"),
					IconExpanded = TfConstants.GetIcon("Pin")
				});
			}
		}
		finally
		{
			UriInitialized = _navState?.Uri ?? String.Empty;
			_initedViewId = spaceView.Id;
		}
	}

	private TfMenuItem _getMenuItem(TfSpaceViewPreset preset, bool isFirst, bool isRoot)
	{
		string? url = null;
		if (!preset.IsGroup)
		{
			if (isFirst && isRoot)
			{
				url = Navigator.GetLocalUrl();
			}
			else
			{
				url = Navigator.GetLocalUrl().ApplyChangeToUrlQuery(TfConstants.PresetIdQueryName,
						preset.Id.ToString());
			}
		}

		var item = new TfMenuItem
		{
			Id = TfConverters.ConvertGuidToHtmlElementId(preset.Id),
			Text = preset.Name,
			Url = url,
			Color = preset.Color,
			IconCollapsed = TfConstants.GetIcon(preset.Icon),
			IconExpanded = TfConstants.GetIcon(preset.Icon)
		};
		if (TfAuthLayout.GetState().User.IsAdmin)
		{
			item.Actions.Add(new TfMenuItem()
			{
				Text = "Manage Preset",
				IconCollapsed = TfConstants.GetIcon("Settings"),
				IconExpanded = TfConstants.GetIcon("Settings"),
				OnClick = EventCallback.Factory.Create(this, async () => await _managePresetHandler(preset))
			});
			item.Actions.Add(new TfMenuItem() { IsDivider = true });
			item.Actions.Add(new TfMenuItem()
			{
				Text = preset.ParentId is null ? "Move left" : "Move up",
				IconCollapsed =
					preset.ParentId is null ? TfConstants.GetIcon("ArrowLeft") : TfConstants.GetIcon("ArrowUp"),
				IconExpanded =
					preset.ParentId is null ? TfConstants.GetIcon("ArrowLeft") : TfConstants.GetIcon("ArrowUp"),
				OnClick = EventCallback.Factory.Create(this, async () => await _movePresetHandler(preset, true))
			});
			item.Actions.Add(new TfMenuItem()
			{
				Text = preset.ParentId is null ? "Move right" : "Move down",
				IconCollapsed =
					preset.ParentId is null ? TfConstants.GetIcon("ArrowRight") : TfConstants.GetIcon("ArrowDown"),
				IconExpanded =
					preset.ParentId is null ? TfConstants.GetIcon("ArrowRight") : TfConstants.GetIcon("ArrowDown"),
				OnClick = EventCallback.Factory.Create(this, async () => await _movePresetHandler(preset, false))
			});
			item.Actions.Add(new TfMenuItem() { IsDivider = true });
			item.Actions.Add(new TfMenuItem()
			{
				Text = "Delete Preset",
				IconCollapsed = TfConstants.GetIcon("Delete")!.WithColor(Color.Error),
				IconExpanded = TfConstants.GetIcon("Settings")!.WithColor(Color.Error),
				OnClick = EventCallback.Factory.Create(this, async () => await _deletePresetHandler(preset))
			});
		}

		foreach (var child in preset.Presets)
		{
			item.Items.Add(_getMenuItem(child, isFirst, false));
		}

		return item;
	}

	private void _setSelection(TfMenuItem menuItem, Guid? currrentPresetId)
	{
		menuItem.Selected = currrentPresetId is not null
							&& menuItem.IdTree.Contains(currrentPresetId.Value.ToString());

		foreach (var child in menuItem.Items)
		{
			_setSelection(child, currrentPresetId);
		}
	}

	private async Task _managePresetHandler(TfSpaceViewPreset preset)
	{
		var context = new TfPresetFilterManagementContext { Item = preset, DateSet = SpaceData, SpaceView = SpaceView };
		var dialog = await DialogService.ShowDialogAsync<TucPresetFilterManageDialog>(
			context,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthExtraLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}


	private async Task _deletePresetHandler(TfSpaceViewPreset preset)
	{
		try
		{
			await TfService.RemoveSpaceViewPreset(SpaceView.Id, preset.Id);
			ToastService.ShowSuccess(LOC("Preset successfully removed!"));
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

	private async Task _movePresetHandler(TfSpaceViewPreset preset, bool isUp)
	{
		try
		{
			await TfService.MoveSpaceViewPreset(SpaceView.Id, preset.Id, isUp);
			ToastService.ShowSuccess(LOC("Preset successfully moved!"));
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
}